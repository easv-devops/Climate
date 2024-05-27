using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers.AlertClientHandlers;

public class ClientWantsToGetAlertsDto : BaseDto
{
    [Required(ErrorMessage = "IsRead boolean is required")]
    public bool IsRead { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetAlerts : BaseEventHandler<ClientWantsToGetAlertsDto>
{
    private readonly AlertService _alertService;

    public ClientWantsToGetAlerts(AlertService alertService)
    {
        _alertService = alertService;
    }

    public override Task Handle(ClientWantsToGetAlertsDto dto, IWebSocketConnection socket)
    {
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User.Id;

        var alerts = _alertService.GetAlertsForUser(userId, dto.IsRead);

        socket.SendDto(new ServerSendsAlertList()
        {
            Alerts = alerts
        });

        return Task.CompletedTask;
    }
}