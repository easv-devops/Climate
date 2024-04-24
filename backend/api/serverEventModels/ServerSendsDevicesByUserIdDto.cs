using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsDevicesByUserIdDto : BaseDto
{
    public IEnumerable<DeviceFullDto>? Devices { get; set; }
}