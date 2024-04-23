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
            connection.Execute(createDeviceQuery, new { DebiceName = deviceDto.DeviceName });

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

    /**
     * Gets all devices from a specific room.
     * Returns a list of devices - can be null if no devices in the room.
     */
    public IEnumerable<DeviceFromRoomDto> GetDevicesByRoomId(int roomId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string getAllQuery = @"
                SELECT Id, DeviceName
                FROM Device 
                WHERE RoomId = @RoomId;";
            return connection.Query<DeviceFromRoomDto>(getAllQuery, new {RoomId = roomId});
        }
        catch (Exception e)
        {
            // Handle exceptions, maybe log them
            throw new SqlTypeException("Failed to retrieve device(s) from room "+roomId, e);
        }
    }

}