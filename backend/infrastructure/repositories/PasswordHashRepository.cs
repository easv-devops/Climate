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
        
        throw new NotImplementedException("Ane du burde lige lave det her");
    }
    
    // Method to test the database connection
    public bool TestConnection()
    {
        using var connection = new MySqlConnection(_connectionString);
        
        try
        {
            connection.Open();
            return true; // Connection successful
        }
        catch (Exception)
        {
            return false; // Connection failed
        }
    }
}