namespace infrastructure.Models;



public class DeviceDto
{
    public required string DeviceName { get; set; }
    public required int? RoomId { get; set; }
}

public class DeviceWithIdDto
{
    public required int Id { get; set; }
    public required string DeviceName { get; set; }
    public required int RoomId { get; set; }
}

public class DeviceByRoomIdDto
{
    public required int Id { get; set; }
    public required string DeviceName { get; set; }
}