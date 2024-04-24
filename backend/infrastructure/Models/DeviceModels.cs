namespace infrastructure.Models;



public class DeviceDto
{
    public required string DeviceName { get; set; }
    public required int RoomId { get; set; }
}

//lav class med ID

public class DeviceWithId
{
    public required string DeviceName { get; set; }
    public required int RoomId { get; set; }
    public required int Id { get; set; }
}