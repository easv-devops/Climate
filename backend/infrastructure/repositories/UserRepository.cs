using System.Data.SqlTypes;
using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    //todo should create a end user with all information including hash and salt 
    public EndUser Create(UserRegisterDto model)
    {
        return new EndUser
        {
            Id = 1,
            Email = "kfe@gmail.com",
            PasswordInfo = new PasswordHash
            {
                Id = 1,
                Hash = "1EJybmIbon7kimzpBZXA17OxI3/iVLZK8euSAloQgK3W8ibEJ8G/Ql2J4kjtDDMRV5sN71LEgRuL+lXyP9dOHz9IuMXuWjTdFSwkKaDNbiNa9MsWy/dngKWo04jYvG8Tb26UV0Bnxd83V9zQZCPdPSQXENoRvPOhnDZKaayFYuRz4pVkBrooL9Hu9EgrCzE9Z3kExf+w1BwR/hqVip2wj+W3mxBwTWgm5hhsko1TZqr3d+HWPAeaFmaNTmwuG0miPhA8H9C4/V0mUs62V2zJkZEVP3QEipvTvkCyctxq7U89NSLwVIGiEsmFG/sZ1EqXnXpmpbV1PQ7pkDYFad+pzQ==",
                Salt = "sMQAck67hWo2asVpqlbmmGVFj3jo6i86oZVTQh3c3wOpKd0LO8oxqSYhveceXkLrXlCKIIVFB+IRPXrcE3ZkFdVKmG5A7gOyvWwkOltwOytSDoPHmT3+aWUS0sFjO89RMbJxCncsghBbtF3a9hHtr/7/NcexUj8wJQz48gq6izw=",
                Algorithm = "argon2id"
            }
        };
    }

    public EndUser GetUserByEmail(string requestEmail)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            // Define the query using joins to fetch all information related to the user
            string query = @"
            SELECT u.Id, u.Email, ui.FirstName, ui.LastName, ci.CountryCode, ci.Number
            FROM User u
            LEFT JOIN UserInformation ui ON u.Id = ui.UserId
            LEFT JOIN ContactInformation ci ON u.Id = ci.UserId
            LEFT JOIN UserStatus us ON u.Id = us.UserId
            WHERE u.Email = @Email";

            // Execute the query using Dapper and retrieve the user information
            var user = connection.QueryFirstOrDefault<EndUser>(query, new { Email = requestEmail });
            
            return user;
        }
        catch (Exception ex)
        {
            // Handle exceptions, maybe log them
            throw new SqlTypeException("Failed to retrieve user by email", ex);
        }
    }
    
    public bool DoesUserExists(string dtoEmail)
    {        
        
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = "SELECT COUNT(*) FROM User WHERE Email = @Email";
            
            int count = connection.ExecuteScalar<int>(query, new { Email = dtoEmail });
            
            // returns true if user exists
            return count > 0;
            
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("failed to search for User", ex);
        }
    }
}