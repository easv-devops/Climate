using System.Data.SqlTypes;
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