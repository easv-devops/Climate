using infrastructure.Models;
using lib;

namespace api.serverEventModels.roomDtos;

public class ServerReturnsAllRooms: BaseDto
{
    public IEnumerable<RoomWithId>? Rooms { get; set; }
}
