using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Security.Authentication;
using System.Security.Cryptography;
using infrastructure;
using infrastructure.Models;
using service.services.Password;

namespace service.services;

public class AuthService
{
    private readonly PasswordHashRepository _passwordHashRepository;
    private readonly UserRepository _userRepository;

    public AuthService(
        UserRepository userRepository,
        PasswordHashRepository passwordHashRepository)
    {
        _userRepository = userRepository;
        _passwordHashRepository = passwordHashRepository;
    }
 
    /**
     * should not be used for returning to frontend!
     */
    public EndUser GetUser(string requestEmail)
    {
        var user = _userRepository.GetUserByEmail(requestEmail);
        if (ReferenceEquals(user, null))
        {
            throw new AuthenticationException("Could not log in");
        }
        user.PasswordInfo = _passwordHashRepository.GetPasswordHashById(user.Id);
        if (ReferenceEquals(user.PasswordInfo, null))
        {
            throw new DataException("Could not find user info");
        }
        return user;
    }

    public bool DoesUserAlreadyExist(string dtoEmail)
    {
        return _userRepository.DoesUserExists(dtoEmail);
    }

    public EndUser RegisterUser(UserRegisterDto model)
    {
        if (DoesUserAlreadyExist(model.Email))
            throw new ValidationException("User Already Exists");

        var user = _userRepository.Create(model); //creates the user 
        if (ReferenceEquals(user, null)) throw new SqlTypeException(" Could not create user");


        user.PasswordInfo = GeneratePasswordHash(user.Id, model.Password);

        var isCreated = _passwordHashRepository.Create(user.PasswordInfo); //stores the password

        if (isCreated == false) throw new SqlTypeException("Could not Create user hash");

        return user;
    }


    public bool ValidateHash(string requestPassword, PasswordHash userPasswordInfo)
    {
        var hashAlgorithm = PasswordHashAlgorithm.Create(userPasswordInfo.Algorithm);
        return hashAlgorithm.VerifyHashedPassword(requestPassword, userPasswordInfo.Hash, userPasswordInfo.Salt);
    }

    public string ResetPassword(string dtoEmail)
    {
        var user = _userRepository.GetUserByEmail(dtoEmail);
        if (ReferenceEquals(user, null)) throw new AuthenticationException("Could Not reset password");

        var newPasswordPlainText = GenerateRandomPassword(9);
        var newPassword = GeneratePasswordHash(user.Id, newPasswordPlainText);
        
        bool isReset = _passwordHashRepository.ReplacePassword(user.Id, newPassword);
        if (!isReset) throw new AuthenticationException("Could Not reset password");
        
        return newPasswordPlainText;
    }

    
    /**
     * Creates a random password for resetting passwords
     */
    private static string GenerateRandomPassword(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        return RandomNumberGenerator.GetString(validChars, length);
    }

    /**
     * Creates a new password hash and returns
     */
    private PasswordHash GeneratePasswordHash(int userId, string passwordString)
    {
        var hashAlgorithm = PasswordHashAlgorithm.Create();
        var salt = hashAlgorithm.GenerateSalt();
        var hash = hashAlgorithm.HashPassword(passwordString, salt);

        var password = new PasswordHash
        {
            Id = userId,
            Hash = hash,
            Salt = salt,
            Algorithm = hashAlgorithm.GetName()
        };
        return password;
    }
    
    public bool TestConnection()
    {
        return _passwordHashRepository.TestConnection();
    }

    public EndUser GetUserById(int userId)
    {
        return _userRepository.GetUserById(userId);
        
    }
}
