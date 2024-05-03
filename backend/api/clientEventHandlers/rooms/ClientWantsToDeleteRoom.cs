using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToDeleteRoomDto : BaseDto
{
    public int RoomId { get; set; }
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
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User!.Id;

        Room room = new Room
        {
            Id = dto.RoomId,
            UserId = userId
        };
        _roomService.DeleteRoom(room);
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