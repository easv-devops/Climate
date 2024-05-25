using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsLatestRoomReadings : BaseDto
{
    public LatestData? Data { get; set; }
}