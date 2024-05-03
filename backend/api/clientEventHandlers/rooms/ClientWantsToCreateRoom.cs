using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToCreateRoomDto : BaseDto
{
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
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User!.Id;

        Room room = new Room
        {
            UserId = userId,
            RoomName = dto.RoomName
        };

        _roomService.CreateRoom(room);
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