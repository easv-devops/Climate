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


    public DeviceDto Create(DeviceDto deviceDto)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string createDeviceQuery = "INSERT INTO Device (DeviceName, RoomId) VALUES (@DeviceName, @RoomId);";
            
            connection.Execute(createDeviceQuery, new { DeviceName = deviceDto.DeviceName, RoomId = deviceDto.RoomId });

            return new DeviceDto
            {
                DeviceName = deviceDto.DeviceName,
                RoomId = deviceDto.RoomId
            };

        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Could not create device", ex);
        }
    }



}