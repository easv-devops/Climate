using infrastructure.Models;
using lib;

namespace api.serverEventModels.rooms;

public class ServerReturnsSpecificRoom : BaseDto
{
    public Room specificRoom { get; set; }
}