using System.Security.Authentication;
using api.helpers;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToEditDeviceSettingsDto : BaseDto
{
    public DeviceSettingsDto DeviceSettings { get; set; }
}

public class ClientWantsToEditDeviceSettingsFromId : BaseEventHandler<ClientWantsToEditDeviceSettingsDto>
{
    private readonly DeviceSettingsService _deviceSettingsService;

    public ClientWantsToEditDeviceSettingsFromId(DeviceSettingsService deviceSettingsService)
    {
        _deviceSettingsService = deviceSettingsService;
    }
    
    public override Task Handle(ClientWantsToEditDeviceSettingsDto dto, IWebSocketConnection socket)
    {
        var users = StateService.GetUsersForDevice(dto.DeviceSettings.DeviceId);
        if (users.Any()) //check if the users has access to device
        {
            throw new AuthenticationException("You do not have access to edit this device");
        }

        var newSettings = _deviceSettingsService.EditSettings(dto.DeviceSettings);

        //return the is deleted bool
        socket.SendDto(new ServerSendsDeviceSettings
        {
            DeviceSettings = newSettings
        });
        
        return Task.CompletedTask;
    }
}

public class ServerSendsDeviceSettings: BaseDto
{
    public DeviceSettingsDto DeviceSettings { get; set; }
}