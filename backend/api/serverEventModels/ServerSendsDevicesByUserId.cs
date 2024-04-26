using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsDevicesByUserId : BaseDto
{
    public IEnumerable<DeviceWithIdDto>? Devices { get; set; }
}