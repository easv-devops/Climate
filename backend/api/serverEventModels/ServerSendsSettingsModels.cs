using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsDeviceRangeSettings: BaseDto
{
    public DeviceRangeDto DeviceSettings { get; set; }
}