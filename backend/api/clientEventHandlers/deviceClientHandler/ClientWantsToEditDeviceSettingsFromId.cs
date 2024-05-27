using System.Security.Authentication;
using api.ClientEventFilters;
using api.helpers;
using api.mqttEventListeners;
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

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToEditDeviceSettingsFromId : BaseEventHandler<ClientWantsToEditDeviceSettingsDto>
{
    private readonly DeviceSettingsService _deviceSettingsService;
    private readonly MqttClientSubscriber _mqttClientSubscriber;


    public ClientWantsToEditDeviceSettingsFromId(DeviceSettingsService deviceSettingsService, MqttClientSubscriber mqttClientSubscriber)
    {
        _deviceSettingsService = deviceSettingsService;
        _mqttClientSubscriber = mqttClientSubscriber;
    }
    
    public override Task Handle(ClientWantsToEditDeviceSettingsDto dto, IWebSocketConnection socket)
    {
        var users = StateService.GetUsersForDevice(dto.Settings.Id);
        if (users.Count == 0) //check if the users has access to device
        {
            throw new AuthenticationException("You do not have access to edit this device");
        }

        var newSettings = _deviceSettingsService.EditSettings(dto.Settings);
        
        //sends the new settings to the device
        _mqttClientSubscriber.SendMessageToBroker(dto.Settings);

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