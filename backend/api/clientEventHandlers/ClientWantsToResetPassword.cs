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
    public override Task Handle(ClientWantsToResetPasswordDto dto, IWebSocketConnection socket)
    {
        string newPassword = _authService.ResetPassword(dto.Email);
        
        _notificationService.SendResetPasswordMessage(MessageType.EMAIL, newPassword, dto.Email);
        return Task.CompletedTask;
    }
}