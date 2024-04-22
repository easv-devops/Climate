using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.security;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using Newtonsoft.Json;
using service.services;
using service.services.notificationServices;

namespace api.clientEventHandlers;  

public class ClientWantsToRegisterDto : BaseDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Phone number is required.")]
    public string Phone { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password is to short.")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "FirstName is required.")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "LastName is required.")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "CountryCode is required.")]
    public string CountryCode { get; set; }
}

[ValidateDataAnnotations]
public class ClientWantsToRegister : BaseEventHandler<ClientWantsToRegisterDto>
{
    private readonly AuthService _authService;

    private readonly TokenService _tokenService;
    private readonly NotificationService _notificationService;
    
    public ClientWantsToRegister(
        AuthService authService,
        TokenService tokenService,
        NotificationService notificationService)
    {
        _authService = authService;
        _tokenService = tokenService;
        _notificationService = notificationService;
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
        
        //sets noti settings and sends welcome message
        List<MessageType> selectedMessageTypes = new List<MessageType>();
        selectedMessageTypes.Add(MessageType.EMAIL);
        _notificationService.SendWelcomeMessage(selectedMessageTypes, new ShortUserDto
        {
            Id = user.Id,
            Email = dto.Email,
            Name = dto.FirstName
        });
        return Task.CompletedTask;
    }
}

