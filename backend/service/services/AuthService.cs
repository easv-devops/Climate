using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Security.Authentication;
using infrastructure;
using infrastructure.Models;
using service.services.Password;

namespace service.services;

public class AuthService
{
    private readonly PasswordHashRepository _passwordHashRepository;
    private readonly UserRepository _userRepository;

    public AuthService(UserRepository userRepository,
        PasswordHashRepository passwordHashRepository)
    {
        _userRepository = userRepository;
        _passwordHashRepository = passwordHashRepository;
    }
    
    public EndUser GetUser(string requestEmail)
    {
        var user = _userRepository.GetUserByEmail(requestEmail);
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

        //chooses hashing algorithm and hashes password
        var hashAlgorithm = PasswordHashAlgorithm.Create();
        var salt = hashAlgorithm.GenerateSalt();
        var hash = hashAlgorithm.HashPassword(model.Password, salt);
        
        var password = new PasswordHash
        {
            Id = user.Id,
            Hash = hash,
            Salt = salt,
            Algorithm = hashAlgorithm.GetName()
        };
        user.PasswordInfo = password;
        
        var isCreated = _passwordHashRepository.Create(password); //stores the password
        if (isCreated == false) throw new SqlTypeException("Could not Create user hash");
        
        return user; 
    }
    

    public bool ValidateHash(string requestPassword, PasswordHash userPasswordInfo)
    {
        var hashAlgorithm = PasswordHashAlgorithm.Create(userPasswordInfo.Algorithm);
        return hashAlgorithm.VerifyHashedPassword(requestPassword, userPasswordInfo.Hash, userPasswordInfo.Salt);
    }
    
    public bool TestConnection()
    {
        return _passwordHashRepository.TestConnection();
    }
}
