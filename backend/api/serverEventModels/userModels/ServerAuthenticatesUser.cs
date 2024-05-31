
using lib;

namespace api.serverEventModels;

public class ServerAuthenticatesUser : BaseDto
{
    public string? Jwt { get; set; }
}