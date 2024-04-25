using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetDevicesByUserIdDto : BaseDto
{
    //Intentionally empty. We need a Dto, but we get the only needed attribute, userId, from the socket connection.
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetDevicesByUserId : BaseEventHandler<ClientWantsToGetDevicesByUserIdDto>
{
    private readonly DeviceService _deviceService;

    public ClientWantsToGetDevicesByUserId(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    public override Task Handle(ClientWantsToGetDevicesByUserIdDto getDevicesByUserIdDto, IWebSocketConnection socket)
    {
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User.Id;
        var devices = _deviceService.GetDevicesByUserId(userId);
        
        socket.SendDto(new ServerSendsDevicesByUserId
        {
            Devices = devices
        });
        
        return Task.CompletedTask;
    }
}