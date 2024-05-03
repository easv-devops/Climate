using infrastructure.Models;
using lib;

namespace api.serverEventModels.rooms;

public class ServerReturnsSpecificRoom : BaseDto
{
    Room specificRoom { get; set; }
}