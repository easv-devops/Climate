using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsLatestRoomReadingsDto : BaseDto
{
    public LatestData? Data { get; set; }
}