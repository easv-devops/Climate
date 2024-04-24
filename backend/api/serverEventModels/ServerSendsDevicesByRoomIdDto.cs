using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsDevicesByRoomIdDto : BaseDto
{
    public required int RoomId { get; set; }
    public IEnumerable<DeviceByRoomIdDto>? Devices { get; set; }
}
