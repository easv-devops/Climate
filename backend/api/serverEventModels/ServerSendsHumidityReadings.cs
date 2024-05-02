using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsHumidityReadings : BaseDto
{
    public IEnumerable<SensorDto> HumidityReadings { get; set; }
}