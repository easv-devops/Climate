using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerReturnsAllRooms : BaseDto
{
    public List<Room> rooms { get; set; }
}
