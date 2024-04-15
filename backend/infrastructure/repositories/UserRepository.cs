using infrastructure.Models;

namespace infrastructure;

public class UserRepository
{
    //todo should create a end user with all information including hash and salt 
    public EndUser Create(UserRegisterDto model)
    {
        return new EndUser
        {
            Id = 1,
            Email = "kfe@gmail.com",
            Isbanned = false
        };
    }

    //todo should return user by email
    public EndUser GetUserByEmail(string requestEmail)
    {
        return new EndUser
        {
            Id = 1,
            Email = "kfe@gmail.com",
            Isbanned = false
        };
    }

    //todo should check if the user exist in db and return true if the user exists 
    public bool DoesUserExists(string dtoEmail)
    {
        throw new NotImplementedException();
    }
}