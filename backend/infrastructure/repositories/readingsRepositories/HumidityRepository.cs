using System.Data.SqlTypes;
using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure.repositories.readingsRepositories;

public class HumidityRepository
{
        
    private readonly string _connectionString;

    public HumidityRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public bool SaveHumidityList(int deviceId, List<SensorDto> dataHumidities)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            foreach (var humidity in dataHumidities)
            {
                Console.WriteLine(humidity.Value);
                Console.WriteLine(humidity.TimeStamp);
                var sql = @"INSERT INTO ReadingHumidity (DeviceId, Timestamp, Humidity) 
                        VALUES (@DeviceId, @Timestamp, @Humidity)";
                connection.Execute(sql, new
                {
                    DeviceId = deviceId,
                    Timestamp = humidity.TimeStamp,
                    Humidity = humidity.Value
                });
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new SqlTypeException("Failed to save Humidity readings", ex);
        }
        
    }
}

