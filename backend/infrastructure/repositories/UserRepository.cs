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

    public IEnumerable<CountryCodeDto> GetCountryCodes()
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
            SELECT CountryCode, Country, IsoCode
            FROM CountryCode;";
            return connection.Query<CountryCodeDto>(query);
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to get country codes", e);
        }
    }
    
    
    public EndUser Create(UserRegisterDto dto)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            
            // Opret brugeren i User-tabellen
            string insertUserQuery = "INSERT INTO User (Email) VALUES (@Email); SELECT LAST_INSERT_ID();";
            int userId = connection.ExecuteScalar<int>(insertUserQuery, new { Email = dto.Email });

            // Opret brugeroplysninger i UserInformation-tabellen
            string insertUserInfoQuery = "INSERT INTO UserInformation (UserId, FirstName, LastName) VALUES (@UserId, @FirstName, @LastName);";
            connection.Execute(insertUserInfoQuery, new { UserId = userId, FirstName = dto.FirstName, LastName = dto.LastName });
            
            Console.WriteLine(dto.CountryCode);
            // Opret kontaktoplysninger i ContactInformation-tabellen
            string insertContactInfoQuery = "INSERT INTO ContactInformation (UserId, IsoCode, Number) VALUES (@UserId, @IsoCode, @Number);";
            connection.Execute(insertContactInfoQuery, new { UserId = userId, IsoCode = dto.CountryCode, Number = dto.Phone });

            // Returner EndUser-objektet
            //todo should set isBanned as false when added on enduser (when isBanned is added to user object)
            return new EndUser
            {
                Id = userId,
                Email = dto.Email
            };
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Could not create user in db.", ex);
        }
    }

    public EndUser GetUserByEmail(string requestEmail)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            // Define the query using joins to fetch all information related to the user
            string query = @"
            SELECT u.Id, u.Email, ui.FirstName, ui.LastName, cc.CountryCode, ci.Number
            FROM User u
            LEFT JOIN UserInformation ui ON u.Id = ui.UserId
            LEFT JOIN ContactInformation ci ON u.Id = ci.UserId
            LEFT JOIN UserStatus us ON u.Id = us.UserId
            LEFT JOIN CountryCode cc ON ci.IsoCode = cc.IsoCode
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

    public EndUser GetUserById(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            // Definér forespørgslen ved hjælp af joins til at hente alle oplysninger relateret til brugeren
            string query = @"
        SELECT u.Id, u.Email, ui.FirstName, ui.LastName, cc.CountryCode, ci.Number
        FROM User u
        LEFT JOIN UserInformation ui ON u.Id = ui.UserId
        LEFT JOIN ContactInformation ci ON u.Id = ci.UserId
        LEFT JOIN UserStatus us ON u.Id = us.UserId
        LEFT JOIN CountryCode cc ON ci.IsoCode = cc.IsoCode
        WHERE u.Id = @UserId";

            // Udfør forespørgslen ved hjælp af Dapper og hent brugeroplysningerne
            var user = connection.QueryFirstOrDefault<EndUser>(query, new { UserId = userId });

            return user;
        }
        catch (Exception ex)
        {
            // Håndter undtagelser, måske log dem
            throw new SqlTypeException("Fejl ved hentning af bruger efter id", ex);
        }
    }
}