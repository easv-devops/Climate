using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsDevicesFromRoom : BaseDto
{
    public required int RoomId { get; set; }
    public IEnumerable<DeviceFromRoomDto>? Devices { get; set; }
}
