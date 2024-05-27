using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers.AlertClientHandlers;

/**
 * This class is only used for testing.
 * Keep ClientWantsToCreateAlert ~identical to MqttClientSubscriber.ScreenReadings
 */


public class ClientWantsToCreateAlertDto : BaseDto
{
    public DeviceData DeviceData { get; set; }
}

public class ClientWantsToCreateAlert : BaseEventHandler<ClientWantsToCreateAlertDto>
{
    private readonly AlertService _alertService;
    public ClientWantsToCreateAlert(AlertService alertService)
    {
        _alertService = alertService;
    }

    public override Task Handle(ClientWantsToCreateAlertDto dto, IWebSocketConnection socket)
    {
        // Screens all readings for values out of range, and creates alerts in db
        var alerts = _alertService.ScreenReadings(dto.DeviceData);
        
        if(alerts.Count == 0)
            return Task.CompletedTask; // No need to send an empty list
        
        // Sends all new alerts to any active clients subscribed to the device that sent readings
        var subscribedUserList = StateService.GetUsersForDevice(dto.DeviceData.DeviceId);

        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetClient(user);
            if (!ReferenceEquals(connection, null))
            {
                connection.Connection.SendDto(new ServerSendsAlertList()
                {
                    Alerts = alerts
                });
            }
        }
        
        return Task.CompletedTask;
    }
}