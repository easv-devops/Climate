using System.ComponentModel.DataAnnotations;
using api.helpers;
using api.security;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToRegisterDto : BaseDto
{
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CountryCode { get; set; }
}


public class ClientWantsToRegister : BaseEventHandler<ClientWantsToRegisterDto>
{
    
    private readonly AuthService _authService;

    private readonly TokenService _tokenService;

    public ClientWantsToRegister(
        AuthService authService,
        TokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }
    public override Task Handle(ClientWantsToRegisterDto dto, IWebSocketConnection socket)
    {
        //check if the user already exists 
        if (_authService.DoesUserAlreadyExist(dto.Email))
            throw new ValidationException("User with this email already exists");
        
        //save the user and password to the db
        EndUser user = _authService.RegisterUser(new UserRegisterDto
        {
            Email = dto.Email,
            Password = dto.Password,
            CountryCode = dto.CountryCode,
            Phone = dto.Phone,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        });
        
        //issue token
        var token = _tokenService.IssueJwt(user.Id);

        //add user information and validates user to state service for later use
        StateService.GetClient(socket.ConnectionInfo.Id).IsAuthenticated = true;
        StateService.GetClient(socket.ConnectionInfo.Id).User = user;
        
        //return JWT to client 
        socket.SendDto(new ServerAuthenticatesUser { Jwt = token });
        return Task.CompletedTask;
    }
}