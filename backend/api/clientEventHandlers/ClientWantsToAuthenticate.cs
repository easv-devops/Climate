using System.Security.Authentication;
using System.Text.Json;
using Fleck;
using api.helpers;
using api.security;
using api.serverEventModels;
using api.WebSocket;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToSignInDto : BaseDto
{
    public string email { get; set; }
    public string password { get; set; }
}

public class ClientWantsToAuthenticate : BaseEventHandler<ClientWantsToSignInDto>
{
    private readonly AuthService _authService;
    private readonly TokenService _tokenService;

    public ClientWantsToAuthenticate(
        AuthService authService,
        TokenService tokenService)
    {
        Console.Write("1");
        _authService = authService;
        _tokenService = tokenService;
    }

    public override Task Handle(ClientWantsToSignInDto request, IWebSocketConnection socket)
    {
        Console.Write("   2");

        //gets user from db and checks for ban status
        var user = _authService.GetUser(request.email);
        if (user.Isbanned) throw new AuthenticationException("User is banned");
        Console.Write("   3");

        //checks password hash
        bool validated = _authService.ValidateHash(request.password!, user.PasswordInfo!);
        if (!validated) throw new AuthenticationException("Wrong credentials!");
        Console.Write("   4");

        //authenticates and sets user information in state service for later use
        StateService.GetClient(socket.ConnectionInfo.Id).IsAuthenticated = true;
        StateService.GetClient(socket.ConnectionInfo.Id).User = user;

        Console.Write("   5");
        //sends the JWT token to the client
        socket.SendDto(new ServerAuthenticatesUser
        {
            Jwt = _tokenService.IssueJwt(new ShortUserDto
            {
                Id = user.Id,
                Email = user.Email
            })
        });
        return Task.CompletedTask;
    }
}