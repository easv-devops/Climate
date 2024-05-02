using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsTemperatureReadings : BaseDto
{
    public IEnumerable<SensorDto> TemperatureReadings { get; set; }
}