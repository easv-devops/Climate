using System.Data.SqlTypes;
using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure.repositories.readingsRepositories;


public class TemperatureRepository
{
    
    private readonly string _connectionString;

    public TemperatureRepository(string connectionString)
    {
        _connectionString = connectionString;
        
    }
    
    public bool SaveTemperatureList(int deviceId, List<SensorDto> tempList)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            foreach (var temperature in tempList)
            {
                var sql = @"INSERT INTO ReadingTemperature (DeviceId, Timestamp, Temperature) 
                        VALUES (@DeviceId, @Timestamp, @Temperature)";
                connection.Execute(sql, new
                {
                    DeviceId = deviceId,
                    Timestamp = temperature.TimeStamp,
                    Temperature = temperature.Value
                });
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Failed to save Temperature readings", ex);
        }
    }
    
    public bool DeleteTemperatureReadings(int deviceId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            var sql = @"DELETE FROM ReadingTemperature 
                    WHERE DeviceId = @DeviceId";
            connection.Execute(sql, new
            {
                DeviceId = deviceId
            });
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new SqlTypeException("Failed to delete Temperature readings", ex);
        }
    }
}