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
    public IEnumerable<DeviceByRoomIdDto> GetDevicesByRoomId(int roomId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string getAllQuery = @"
                SELECT Id, DeviceName
                FROM Device 
                WHERE RoomId = @RoomId;";
            return connection.Query<DeviceByRoomIdDto>(getAllQuery, new {RoomId = roomId});
        }
        catch (Exception e)
        {
            // Handle exceptions, maybe log them
            throw new SqlTypeException("Failed to retrieve device(s) from room "+roomId, e);
        }
    }
    
    /**
     * Gets all devices for logged in user.
     * Returns a list of devices - can be null if user has no devices.
     */
    public IEnumerable<DeviceFullDto> GetDevicesByUserId(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string getAllQuery = @"
                SELECT Device.Id, DeviceName, Device.RoomId
                FROM Device 
                JOIN Room ON Device.RoomId = Room.Id
                WHERE Room.UserId = @UserId;";
            return connection.Query<DeviceFullDto>(getAllQuery, new {UserId = userId});
        }
        catch (Exception e)
        {
            // Handle exceptions, maybe log them
            throw new SqlTypeException("Failed to retrieve device(s) for user with id "+userId, e);
        }
    }
    
    /**
     * Gets device from device id.
     * Returns a device.
     */
    public DeviceFullDto GetDeviceById(int deviceId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string getDeviceByIdQuery = @"
                SELECT *
                FROM Device 
                WHERE Id = @DeviceId;";
            return connection.QueryFirstOrDefault<DeviceFullDto>(getDeviceByIdQuery, new {DeviceId = deviceId}) ?? throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            // Handle exceptions, maybe log them
            throw new SqlTypeException("Failed to retrieve device with id "+deviceId, e);
        }
    }

}