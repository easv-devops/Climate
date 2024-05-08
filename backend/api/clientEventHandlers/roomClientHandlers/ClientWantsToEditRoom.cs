using System.Net.Sockets;
using System.Security.Authentication;
using api.ClientEventFilters;
using api.ServerEventHandlers;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers.roomClientHandlers;

public class ClientWantsToEditRoomDto : BaseDto
{
    public required RoomWithId RoomToEdit { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToEditRoom: BaseEventHandler<ClientWantsToEditRoomDto>
{
    private readonly RoomService _roomService;
    private readonly ServerWantsToSendRoom _serverResponse;

    public ClientWantsToEditRoom(RoomService roomService, ServerWantsToSendRoom serverResponse)
    {
        _roomService = roomService;
        _serverResponse = serverResponse;
    }
    
    public override Task Handle(ClientWantsToEditRoomDto dto, IWebSocketConnection socket)
    {
        var users =StateService.GetUsersForRoom(dto.RoomToEdit.Id);

        if (!users.Contains(socket.ConnectionInfo.Id))
        {
            throw new AuthenticationException("You do not have permission to edit room");
        }

        RoomWithId room = _roomService.EditRoom(dto.RoomToEdit);
        
        _serverResponse.SendRoomToClient(room);
        
        return Task.CompletedTask;
    }
}