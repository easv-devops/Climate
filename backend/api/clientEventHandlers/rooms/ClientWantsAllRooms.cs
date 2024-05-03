using api.helpers;
using api.serverEventModels;
using api.serverEventModels.rooms;
using api.WebSocket;
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
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User!.Id;
        List<Room> allrooms = _roomService.GetAllRooms(userId);
        socket.SendDto(
            new ServerReturnsAllRooms()
            {
                rooms = allrooms
            }
        );

        return Task.CompletedTask;
    }
}