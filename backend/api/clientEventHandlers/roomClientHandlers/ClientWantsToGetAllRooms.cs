using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels.roomDtos;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers.roomClientHandlers;

public class ClientWantsToGetAllRoomsDto : BaseDto
{
    //Intentionally empty. We need a Dto, but we get the only needed attribute, userId, from the socket connection.
}

[RequireAuthentication]
public class ClientWantsToGetAllRooms:  BaseEventHandler<ClientWantsToGetAllRoomsDto>
{
    private readonly RoomService _roomService;

    public ClientWantsToGetAllRooms(RoomService roomService)
    {
        _roomService = roomService;
    }
    
    
    public override Task Handle(ClientWantsToGetAllRoomsDto dto, IWebSocketConnection socket)
    {
        var user = StateService.GetConnection(socket.ConnectionInfo.Id);
        
        var roomList = _roomService.GetAllRooms(user.User!.Id);
        
        socket.SendDto(new ServerReturnsAllRooms
        {
            Rooms = roomList
        });


        foreach (var room in roomList)
        {
            StateService.AddConnectionToRoom(room.Id, user.Connection.ConnectionInfo.Id);
        }
        
        return Task.CompletedTask;
    }
}