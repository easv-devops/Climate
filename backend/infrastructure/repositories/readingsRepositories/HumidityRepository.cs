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
            throw new SqlTypeException("Failed to save Humidity readings", ex);
        }
    }
    
    public bool DeleteHumidityReadings(int deviceId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            var sql = @"DELETE FROM ReadingHumidity 
                    WHERE DeviceId = @DeviceId";
            connection.Execute(sql, new
            {
                DeviceId = deviceId
            });
            return true;
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Failed to delete Humidity readings", ex);
        }
    }
    
    public IEnumerable<SensorDto> GetHumidityReadingsFromDevice(int deviceId, DateTime? startTime, DateTime? endTime)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
        
            var sql = @"
            SELECT 
                Timestamp AS TimeStamp,
                Humidity AS Value 
            FROM 
                ReadingHumidity 
            WHERE 
                DeviceId = @deviceId
                AND Timestamp >= @startTime
                AND Timestamp <= @endTime
            ORDER BY 
                Timestamp;
        ";

            return connection.Query<SensorDto>(sql, new { DeviceId = deviceId, StartTime = startTime, EndTime = endTime });
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to retrieve humidity readings from device " + deviceId, e);
        }
    }
}

