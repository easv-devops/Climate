using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsLatestDeviceReadingsDto : BaseDto
{
    public LatestDeviceData? Data { get; set; }
}