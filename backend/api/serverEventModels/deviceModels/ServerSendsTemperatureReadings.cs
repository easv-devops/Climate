using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsTemperatureReadings : BaseDto, IDeviceReadingsDto
{
    public int DeviceId { get; set; }
    public IEnumerable<SensorDto>? Readings { get; set; }
}