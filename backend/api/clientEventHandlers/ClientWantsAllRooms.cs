using api.helpers;
using api.serverEventModels;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsAllRoomsDto : BaseDto
{
}

public class ClientWantsAllRooms : BaseEventHandler<ClientWantsAllRoomsDto>
{
    
    private readonly RoomService _roomService;

    public ClientWantsAllRooms(RoomService roomService)
    {
        _roomService = roomService;
    }

    public override Task Handle(ClientWantsAllRoomsDto dto, IWebSocketConnection socket)
    {
        List<Room> allrooms = _roomService.GetAllRooms();
        
        socket.SendDto(
            new ServerReturnsAllRooms()
            {
                rooms = allrooms
            }
        );

        return Task.CompletedTask;
    }
}