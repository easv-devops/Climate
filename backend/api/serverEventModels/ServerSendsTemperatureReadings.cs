using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsTemperatureReadings : BaseDto
{
    public int DeviceId { get; set; }
    public IEnumerable<SensorDto>? TemperatureReadings { get; set; }
}