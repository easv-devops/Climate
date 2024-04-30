
using lib;

namespace api.serverEventModels;


public class ServerSendsErrorMessageToClient : BaseDto
{
    public string? errorMessage { get; set; }
    public string? receivedMessage { get; set; }
}