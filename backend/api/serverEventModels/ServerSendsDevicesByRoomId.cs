using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsDevicesByRoomId : BaseDto
{
    public required int RoomId { get; set; }
    public IEnumerable<DeviceByRoomIdDto>? Devices { get; set; }
}
