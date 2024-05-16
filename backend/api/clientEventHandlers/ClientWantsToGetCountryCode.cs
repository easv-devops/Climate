using api.helpers;
using api.serverEventModels;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetCountryCodeDto : BaseDto
{
    //Intentionally empty.
}

public class ClientWantsToGetCountryCode : BaseEventHandler<ClientWantsToGetCountryCodeDto>
{
    private readonly UserService _userService;

    public ClientWantsToGetCountryCode(UserService userService)
    {
        _userService = userService;
    }
    
    public override Task Handle(ClientWantsToGetCountryCodeDto dto, IWebSocketConnection socket)
    {
        var countryCodes =
            _userService.GetCountryCodes();
        
        socket.SendDto(new ServerSendsCountryCodes
        {
            CountryCode = countryCodes
        });
        
        return Task.CompletedTask;
    }
}

