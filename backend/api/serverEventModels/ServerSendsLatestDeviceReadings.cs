using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsLatestDeviceReadings : BaseDto
{
    public LatestData? Data { get; set; }
}