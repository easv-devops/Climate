using infrastructure.Models;
using lib;

namespace api.serverEventModels.roomDtos;

public class ServerSendsTemperatureReadingsForRoom : BaseDto
{
    public int RoomId { get; set; }
    public IEnumerable<SensorDto>? TemperatureReadings { get; set; }
}