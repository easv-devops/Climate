namespace infrastructure.Models;



public class DeviceDto
{
    public required string DeviceName { get; set; }
    public required int RoomId { get; set; }
}

public class DeviceFromRoomDto
{
    public required int Id { get; set; }
    public required string DeviceName { get; set; }
}