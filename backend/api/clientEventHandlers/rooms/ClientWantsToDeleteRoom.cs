using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToDeleteRoomDto : BaseDto
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
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
        Room room = new Room
        {
            Id = dto.RoomId,
            UserId = dto.UserId
        };
        _roomService.DeleteRoom(room);
        return Task.CompletedTask;
    }
}