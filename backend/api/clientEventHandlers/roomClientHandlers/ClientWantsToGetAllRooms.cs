using System.Net.Sockets;
using System.Security.Authentication;
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

public class ClientWantsToGetAllRooms:  BaseEventHandler<ClientWantsToGetAllRoomsDto>
{
    private RoomService _roomService;

    public ClientWantsToGetAllRooms(RoomService roomService)
    {
        _roomService = roomService;
    }
    
    
    public override Task Handle(ClientWantsToGetAllRoomsDto dto, IWebSocketConnection socket)
    {
        var user = StateService.GetClient(socket.ConnectionInfo.Id);

 
        
        var roomList = _roomService.GetAllRooms(user.User!.Id);
        
        socket.SendDto(new ServerReturnsAllRooms
        {
            Rooms = roomList
        });


        foreach (var room in roomList)
        {
            StateService.AddUserToRoom(room.Id, user.Connection.ConnectionInfo.Id);
        }
        
        return Task.CompletedTask;
    }
}