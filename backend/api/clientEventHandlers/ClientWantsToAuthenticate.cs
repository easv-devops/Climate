using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToSignInDto : BaseDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    public string email { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password is to short.")]
    public string password { get; set; }
}
[ValidateDataAnnotations]
public class ClientWantsToAuthenticate : BaseEventHandler<ClientWantsToSignInDto>
{
    private readonly AuthService _authService;
    private readonly TokenService _tokenService;

    public ClientWantsToAuthenticate(
        AuthService authService,
        TokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    public override async Task Handle(ClientWantsToSignInDto request, IWebSocketConnection socket)
    {
        //gets user information from db and checks for ban status
        var user = _authService.GetUser(request.email);
        //if (user.Isbanned) throw new AuthenticationException("User is banned");
        
        //checks password hash
        bool validated = _authService.ValidateHash(request.password!, user.PasswordInfo!);
        if (!validated) throw new AuthenticationException("Wrong credentials!");

        //authenticates and sets user information in state service for later use
        StateService.GetClient(socket.ConnectionInfo.Id).IsAuthenticated = true;
        StateService.GetClient(socket.ConnectionInfo.Id).User = user;

        //sends the JWT token to the client
        socket.SendDto(new ServerAuthenticatesUser
        {
            Jwt =  await _tokenService.IssueJwt(user.Id)
        });
    }
}