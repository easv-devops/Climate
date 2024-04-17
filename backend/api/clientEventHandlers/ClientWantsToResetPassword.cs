using Fleck;
using lib;

namespace api.clientEventHandlers;


public class ClientWantsToResetPasswordDto : BaseDto
{
    public string Email { get; set; }
}





public class ClientWantsToResetPassword : BaseEventHandler<ClientWantsToResetPasswordDto>
{
    public override Task Handle(ClientWantsToResetPasswordDto dto, IWebSocketConnection socket)
    {
        throw new NotImplementedException();
    }
}