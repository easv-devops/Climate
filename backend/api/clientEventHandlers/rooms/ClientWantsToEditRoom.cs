using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToEditRoomDto : BaseDto
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public string RoomName { get; set; }
}

public class ClientWantsToEditRoom : BaseEventHandler<ClientWantsToEditRoomDto>
{
    
    private readonly RoomService _roomService;

    public ClientWantsToEditRoom(RoomService roomService)
    {
        _roomService = roomService;
    }
    
    public override Task Handle(ClientWantsToEditRoomDto dto, IWebSocketConnection socket)
    {
        Room room = new Room()
        {
            Id = dto.RoomId,
            UserId = dto.UserId,
            RoomName = dto.RoomName
        };
        Room editRoom = _roomService.EditRoom(room);
        
        return Task.CompletedTask;
    }
}