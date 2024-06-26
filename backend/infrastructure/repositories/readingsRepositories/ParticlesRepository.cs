﻿using System.Data.SqlTypes;
using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure.repositories.readingsRepositories;

public class ParticlesRepository
{
    private readonly string _connectionString;

    public ParticlesRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public bool SaveParticle25List(int deviceId, List<SensorDto> particles25List)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            foreach (var particle in particles25List)
            {
                var sql = @"INSERT INTO ReadingParticle2_5 (DeviceId, Timestamp, P2_5) 
                        VALUES (@DeviceId, @Timestamp, @P2_5)";
                connection.Execute(sql, new
                {
                    DeviceId = deviceId,
                    Timestamp = particle.TimeStamp,
                    P2_5 = particle.Value
                });
            }
            return true;
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Failed to save Particle25 readings", ex);
        }
    }
    
    public bool DeleteParticle25(int deviceId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            var sql = @"DELETE FROM ReadingParticle2_5 
                    WHERE DeviceId = @DeviceId";
            connection.Execute(sql, new
            {
                DeviceId = deviceId
            });
            return true;
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Failed to delete Particle25 readings", ex);
        }
    }
    
    public bool SaveParticle100List(int deviceId, List<SensorDto> particles100List)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            foreach (var particle in particles100List)
            {
                var sql = @"INSERT INTO ReadingParticle10 (DeviceId, Timestamp, P10) 
                        VALUES (@DeviceId, @Timestamp, @P10)";
                connection.Execute(sql, new
                {
                    DeviceId = deviceId,
                    Timestamp = particle.TimeStamp,
                    P10 = particle.Value
                });
            }
            return true;
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Failed to save Particle100 readings", ex);
        }
    }
    
    public bool DeleteParticle100(int deviceId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            var sql = @"DELETE FROM ReadingParticle10 
                    WHERE DeviceId = @DeviceId";
            connection.Execute(sql, new
            {
                DeviceId = deviceId
            });
            return true;
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Failed to delete Particle100 readings", ex);
        }
    }
    
    public IEnumerable<SensorDto> GetPm25ReadingsFromDevice(int deviceId, DateTime? startTime, DateTime? endTime)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            var sql = @"
            SELECT 
                Timestamp AS TimeStamp,
                P2_5 AS Value 
            FROM 
                ReadingParticle2_5 
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
            throw new SqlTypeException("Failed to retrieve PM2.5 readings from device " + deviceId, e);
        }
    }

    

    
    public IEnumerable<SensorDto> GetPm100ReadingsFromDevice(int deviceId, DateTime? startTime, DateTime? endTime)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            var sql = @"
            SELECT 
                Timestamp AS TimeStamp,
                P10 AS Value 
            FROM 
                ReadingParticle10 
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
            throw new SqlTypeException("Failed to retrieve PM10 readings from device " + deviceId, e);
        }
    }

    public SensorDto GetLatestP25ReadingFromDevice(int deviceId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
        
            var sql = @"
                SELECT 
                    Timestamp AS TimeStamp,
                    P2_5 AS Value
                FROM 
                    ReadingParticle2_5 
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
            throw new SqlTypeException("Failed to retrieve the latest PM2.5 reading from device " + deviceId, e);
        }
    }
    
    public SensorDto GetLatestP100ReadingFromDevice(int deviceId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
        
            var sql = @"
                SELECT 
                    Timestamp AS TimeStamp,
                    P10 AS Value
                FROM 
                    ReadingParticle10 
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
            throw new SqlTypeException("Failed to retrieve the latest PM10 reading from device " + deviceId, e);
        }
    }
}