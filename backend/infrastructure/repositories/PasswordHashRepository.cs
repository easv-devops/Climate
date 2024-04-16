using System.Data.SqlTypes;
using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure;

public class PasswordHashRepository
{
    private readonly string _connectionString;
    
    public PasswordHashRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    //todo should create in db
    public bool Create(PasswordHash password)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("failed to Save Password", ex);
        }

        return true;
    }
    
    public PasswordHash GetPasswordHashById(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            // Define the query using joins to fetch all information related to the user
            string query = @"
            SELECT *
            FROM PasswordHash
            WHERE UserId = @UserId";

            // Execute the query using Dapper and retrieve the user information
            var pwHash = connection.QueryFirstOrDefault<PasswordHash>(query, new { UserId = userId });
            
            return pwHash;
        }
        catch (Exception ex)
        {
            // Handle exceptions, maybe log them
            throw new SqlTypeException("Failed to retrieve password hash by email", ex);
        }
    }
    
    // Method to test the database connection
    //todo should be deleted when other ways of testing is done
    public bool TestConnection()
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            
            return true; // Connection successful
        }
        catch (Exception e)
        {
         
            return false; // Connection failed
        }
    }
}