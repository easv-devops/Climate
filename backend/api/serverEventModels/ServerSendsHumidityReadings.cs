using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsHumidityReadings : BaseDto
{
    public int DeviceId { get; set; }
    public IEnumerable<SensorDto>? HumidityReadings { get; set; }
}