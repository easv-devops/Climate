﻿using System.Data.SqlTypes;
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
            throw new SqlTypeException("Failed to delete Temperature readings", ex);
        }
    }

    
    public IEnumerable<SensorDto> GetTemperatureReadingsFromDevice(int deviceId, DateTime? startTime, DateTime? endTime)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            
            var sql = @"
            SELECT 
                TIMESTAMP AS TimeStamp,
                Temperature AS Value
            FROM 
                ReadingTemperature 
            WHERE 
                DeviceId = @deviceId
                AND Timestamp >= @startTime
                AND Timestamp <= @endTime
            ORDER BY 
                Timestamp;
        ";

            return connection.Query<SensorDto>(sql, new { DeviceId = deviceId, StartTime = startTime, EndTime = endTime});
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to retrieve temperature readings from device " + deviceId, e);
        }
    }
    
    public SensorDto GetLatestTemperatureReadingFromDevice(int deviceId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
        
            var sql = @"
                SELECT 
                    Timestamp AS TimeStamp,
                    Temperature AS Value
                FROM 
                    ReadingTemperature 
                WHERE 
                    DeviceId = @deviceId
                ORDER BY 
                    Timestamp DESC
                LIMIT 1;
            ";

            return connection.QueryFirstOrDefault<SensorDto>(sql, new { DeviceId = deviceId });
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to retrieve the latest temperature reading from device " + deviceId, e);
        }
    }

}