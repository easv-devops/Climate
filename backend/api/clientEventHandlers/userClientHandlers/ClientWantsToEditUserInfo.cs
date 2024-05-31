using System.Security.Authentication;
using api.ClientEventFilters;
using api.ServerEventHandlers;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToEditUserInfoDto : BaseDto
{
    public required FullUserDto UserDto { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToEditUserInfo : BaseEventHandler<ClientWantsToEditUserInfoDto>
{
    private readonly UserService _userService;
    private readonly ServerWantsToSendUser _serverResponse;

    public ClientWantsToEditUserInfo(UserService userService, ServerWantsToSendUser serverWantsToSendUser)
    {
        _userService = userService;
        _serverResponse = serverWantsToSendUser;
    }

    public override Task Handle(ClientWantsToEditUserInfoDto dto, IWebSocketConnection socket)
    {
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User.Id;

        if (!Equals(userId, dto.UserDto.Id))
        {
            throw new AuthenticationException("You do not have access to edit this user");
        }

        var user = _userService.EditUser(dto.UserDto);
        
        _serverResponse.SendUserToClient(user);

        return Task.CompletedTask;
    }
}