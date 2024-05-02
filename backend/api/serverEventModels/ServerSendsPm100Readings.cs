using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsPm100Readings : BaseDto
{
    public IEnumerable<SensorDto> Pm100Readings { get; set; }
}