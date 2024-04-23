using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsDevicesByUserId : BaseDto
{
    public IEnumerable<DeviceFullDto>? Devices { get; set; }
}