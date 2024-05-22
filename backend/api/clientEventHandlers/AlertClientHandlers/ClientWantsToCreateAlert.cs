using System.Security.Authentication;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers.AlertClientHandlers;

public class ClientWantsToCreateAlertDto : BaseDto
{
    public DeviceData DeviceData { get; set; }
}

[RequireAuthentication]
public class ClientWantsToCreateAlert : BaseEventHandler<ClientWantsToCreateAlertDto>
{
    private readonly AlertService _alertService;
    public ClientWantsToCreateAlert(AlertService alertService)
    {
        _alertService = alertService;
    }

    public override Task Handle(ClientWantsToCreateAlertDto dto, IWebSocketConnection socket)
    {
        var users = StateService.GetUsersForDevice(dto.DeviceData.DeviceId);
        if (users.Count == 0) //check if the users has access to device
        {
            throw new AuthenticationException("You do not have access to create alerts for this device");
        }
        
        var alerts = _alertService.ScreenReadings(dto.DeviceData);
        
        socket.SendDto(new ServerSendsAlertList()
        {
            Alerts = alerts
        });
        
        return Task.CompletedTask;
    }
}