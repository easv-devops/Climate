using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsLatestDeviceReadingsDto : BaseDto
{
    public LatestData? Data { get; set; }
}