using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;
using service.services.notificationServices;

namespace api.clientEventHandlers;


public class ClientWantsToResetPasswordDto : BaseDto
{
    public string Email { get; set; }
}


[ValidateDataAnnotations]
public class ClientWantsToResetPassword : BaseEventHandler<ClientWantsToResetPasswordDto>
{
    private readonly AuthService _authService;
    private readonly NotificationService _notificationService;
    
    public ClientWantsToResetPassword(AuthService authService,
        NotificationService notificationService)
    {
        _authService = authService;
        _notificationService = notificationService;
    }
    
    //todo should maybe have some security question Like (What is your username or phone number...)
    public override async Task Handle(ClientWantsToResetPasswordDto dto, IWebSocketConnection socket)
    {
        string newPassword = _authService.ResetPassword(dto.Email);
        
        var isReset = _notificationService.SendResetPasswordMessage(MessageType.EMAIL, newPassword, dto.Email);
        socket.SendDto(new ServerResetsPassword { IsReset=  await isReset});
 
    }
}

public class ServerResetsPassword : BaseDto
{
    [Required(ErrorMessage = "No status for reset of password")]
    public bool IsReset { get; set; }
    
}