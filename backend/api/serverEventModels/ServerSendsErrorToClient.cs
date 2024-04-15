
using lib;

namespace infrastructure.Models.serverEvents;


public class ServerSendsErrorMessageToClient : BaseDto
{
    public string? errorMessage { get; set; }
    public string? receivedMessage { get; set; }
}