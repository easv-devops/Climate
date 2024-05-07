using System.Net.Sockets;
using api.ServerEventHandlers;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers.roomClientHandlers;

public class ClientWantsToCreateRoomDto : BaseDto
{
    public CreateRoomDto RoomToCreate { get; set; }
}



public class ClientWantsToCreateRoom: BaseEventHandler<ClientWantsToCreateRoomDto>
{
    
    private RoomService _roomService;
    private ServerWantsToSendRoom _serverResponse;

    public ClientWantsToCreateRoom(RoomService roomService, ServerWantsToSendRoom serverResponse)
    {
        _roomService = roomService;
        _serverResponse = serverResponse;
    }
    
    public override Task Handle(ClientWantsToCreateRoomDto dto, IWebSocketConnection socket)
    {
        var room = _roomService.CreateRoom(dto.RoomToCreate);

        StateService.AddUserToRoom(room.Id, socket.ConnectionInfo.Id);
        
        _serverResponse.SendRoomToClient(room);
        
        return Task.CompletedTask;
    }
}