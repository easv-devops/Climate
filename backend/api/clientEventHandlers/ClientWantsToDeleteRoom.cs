using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToDeleteRoomDto : BaseDto
{
    public Room room;
}

public class ClientWantsToDeleteRoom : BaseEventHandler<ClientWantsToDeleteRoomDto>
{
    private readonly RoomService _roomService;

    public ClientWantsToDeleteRoom(RoomService roomService)
    {
        _roomService = roomService;
    }
    
    public override Task Handle(ClientWantsToDeleteRoomDto dto, IWebSocketConnection socket)
    {
        _roomService.DeleteRoom(dto.room);
        return Task.CompletedTask;
    }
}