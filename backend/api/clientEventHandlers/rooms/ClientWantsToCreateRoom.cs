using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToCreateRoomDto : BaseDto
{
    public int UserId { get; set; }
    public string RoomName { get; set; }
}


public class ClientWantsToCreateRoom : BaseEventHandler<ClientWantsToCreateRoomDto>
{
    
    private readonly RoomService _roomService;

    public ClientWantsToCreateRoom(RoomService roomService)
    {
        _roomService = roomService;
    }

    public override Task Handle(ClientWantsToCreateRoomDto dto, IWebSocketConnection socket)
    {
        Room room = new Room
        {
            UserId = dto.UserId,
            RoomName = dto.RoomName
        };
        Console.WriteLine("userId: " + dto.UserId + " roomname: " + dto.RoomName);

        _roomService.CreateRoom(room);
        return Task.CompletedTask;
    }
}