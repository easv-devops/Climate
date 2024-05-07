using System.Net.Sockets;
using api.ClientEventFilters;
using api.ServerEventHandlers;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers.roomClientHandlers;

public class ClientWantsToCreateRoomDto : BaseDto
{
    public required RoomWithNoId RoomToCreate { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
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
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User!.Id;

        var room = new CreateRoomDto()
        {
            RoomName = dto.RoomToCreate.RoomName,
            UserId = userId
        };
        
        var createdRoom = _roomService.CreateRoom(room);

        StateService.AddUserToRoom(createdRoom.Id, socket.ConnectionInfo.Id);
        
        _serverResponse.SendRoomToClient(createdRoom);
        
        return Task.CompletedTask;
    }
}