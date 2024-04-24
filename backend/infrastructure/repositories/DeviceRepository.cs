using System.Data.SqlTypes;
using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure;

public class DeviceRepository
{
    
    private readonly string _connectionString;

    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }


    public DeviceWithIdDto Create(DeviceDto deviceDto)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string createDeviceQuery = @"
                INSERT INTO Device (DeviceName, RoomId) 
                VALUES (@DeviceName, @RoomId)
                RETURNING *;";
            
            //Console.WriteLine("før conn.query");

            var createdDevice = connection.QueryFirst<DeviceWithIdDto>(createDeviceQuery, new { DeviceName = deviceDto.DeviceName, RoomId = deviceDto.RoomId });

            Console.WriteLine("deviceId: " + createdDevice.Id + ", " + createdDevice.DeviceName);
            //Console.WriteLine("før return");
            return new DeviceWithIdDto
            {
                DeviceName = createdDevice.DeviceName,
                RoomId = createdDevice.RoomId,
                Id = createdDevice.Id
            };
            Console.WriteLine(createdDevice.Id);
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Could not create device", ex);
        }
        
    }



}