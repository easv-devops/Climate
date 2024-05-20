using System.Security.Authentication;
using api.helpers;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToEditDeviceRangeDto : BaseDto
{
    public DeviceRangeDto DeviceSettings { get; set; }
}

public class ClientWantsToEditRangeSettingsForDevice: BaseEventHandler<ClientWantsToEditDeviceRangeDto>
{
    private readonly DeviceSettingsService _deviceSettingsService;

    public ClientWantsToEditRangeSettingsForDevice(DeviceSettingsService deviceSettingsService)
    {
        _deviceSettingsService = deviceSettingsService;
    }


    public override Task Handle(ClientWantsToEditDeviceRangeDto dto, IWebSocketConnection socket)
    {
        var users = StateService.GetUsersForDevice(dto.DeviceSettings.DeviceId);
        if (users.Any()) //check if the users has access to device
        {
            throw new AuthenticationException("You do not have access to edit this device");
        }

        var newSettings = _deviceSettingsService.EditRangeSettings(dto.DeviceSettings);

        //return the is deleted bool
        socket.SendDto(new ServerSendsDeviceRangeSettings
        {
           DeviceSettings = newSettings
        });
        
        return Task.CompletedTask;
    }
}


public class ServerSendsDeviceRangeSettings: BaseDto
{
    public DeviceRangeDto DeviceSettings { get; set; }
}