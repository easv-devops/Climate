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
    public SettingsDto Settings { get; set; }
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
        var users = StateService.GetUsersForDevice(dto.Settings.Id);
        if (!users.Any()) //check if the users has access to device
        {
            throw new AuthenticationException("You do not have access to edit this device");
        }

        var newSettings = _deviceSettingsService.EditSettings(dto.Settings);

        //return the is deleted bool
        socket.SendDto(new ServerSendsDeviceSettings
        {
            Settings = newSettings
        });
        
        return Task.CompletedTask;
    }
}

public class ServerSendsDeviceSettings: BaseDto
{
    public SettingsDto Settings { get; set; }
}