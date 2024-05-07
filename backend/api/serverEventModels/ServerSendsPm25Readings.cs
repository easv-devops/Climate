using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsPm25Readings : BaseDto
{
    public int DeviceId { get; set; }
    public IEnumerable<SensorDto>? Pm25Readings { get; set; }
}