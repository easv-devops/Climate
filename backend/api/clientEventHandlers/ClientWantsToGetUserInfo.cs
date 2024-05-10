using api.ClientEventFilters;
using api.helpers;
using api.ServerEventHandlers;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetUserInfoDto : BaseDto
{
    //Intentionally empty. We need a Dto, but we get the only needed attribute, userId, from the socket connection.
}

[RequireAuthentication]
public class ClientWantsToGetUserInfo : BaseEventHandler<ClientWantsToGetUserInfoDto>
{
    private readonly AuthService _authService;
    private readonly ServerWantsToSendUser _serverResponse;
    
    public ClientWantsToGetUserInfo(AuthService authService, ServerWantsToSendUser serverWantsToSendUser)
    {
        _authService = authService;
        _serverResponse = serverWantsToSendUser;
    }

    public override Task Handle(ClientWantsToGetUserInfoDto dto, IWebSocketConnection socket)
    {
        var userId = StateService.GetConnection(socket.ConnectionInfo.Id).User.Id;

        //gets user information from db and checks for ban status
        var user = _authService.GetFullUserById(userId);
        
        StateService.AddConnectionToUser(user.Id, socket.ConnectionInfo.Id);

        _serverResponse.SendUserToClient(user);
        
        return Task.CompletedTask;
    }
}