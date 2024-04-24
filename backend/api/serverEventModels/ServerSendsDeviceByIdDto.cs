using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsDeviceByIdDto : BaseDto
{
    public required DeviceFullDto Device { get; set; }
}