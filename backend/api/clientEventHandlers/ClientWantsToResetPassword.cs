using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;


public class ClientWantsToResetPasswordDto : BaseDto
{
    public string Email { get; set; }
}

public class ClientWantsToResetPassword : BaseEventHandler<ClientWantsToResetPasswordDto>
{
    private readonly AuthService _authService;
    

    public ClientWantsToResetPassword(AuthService authService)
    {
        _authService = authService;
    }
    
    //todo should maybe have some security question Like (What is your username or phone number...)
    public override Task Handle(ClientWantsToResetPasswordDto dto, IWebSocketConnection socket)
    {
        string newPassword = _authService.ResetPassword(dto.Email);
        
        //todo should call a Notification Service that we can make cool email and maybe sms for the user
        //todo should take the new password and a method of communication emun like (sms, email etc...)
        
        //todo return a Server to client object that holds a bool is reset was a success.
        return Task.CompletedTask;
    }
}