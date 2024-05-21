using System.Security.Authentication;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToEditDeviceRangeDto : BaseDto
{
    public RangeDto Settings { get; set; }
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
        var users = StateService.GetUsersForDevice(dto.Settings.Id);
        if (!users.Any()) //check if the users has access to device
        {
            throw new AuthenticationException("You do not have access to edit this device");
        }

        var newSettings = _deviceSettingsService.EditRangeSettings(dto.Settings);

        //return the is deleted bool
        socket.SendDto(new ServerSendsDeviceRangeSettings
        {
           Settings = newSettings
        });
        
        return Task.CompletedTask;
    }
}

