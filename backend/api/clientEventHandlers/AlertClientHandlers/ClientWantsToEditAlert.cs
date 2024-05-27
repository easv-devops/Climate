using System.Security.Authentication;
using api.ClientEventFilters;
using api.ServerEventHandlers;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers.AlertClientHandlers;

public class ClientWantsToEditAlertDto : BaseDto
{
    public required int AlertId { get; set; }
    public required int DeviceId { get; set; }
    public required bool IsRead { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToEditAlert : BaseEventHandler<ClientWantsToEditAlertDto>
{
    private readonly AlertService _alertService;
    private readonly ServerWantsToSendAlert _serverResponse;

    public ClientWantsToEditAlert(AlertService alertService, ServerWantsToSendAlert serverResponse)
    {
        _alertService = alertService;
        _serverResponse = serverResponse;
    }

    public override Task Handle(ClientWantsToEditAlertDto dto, IWebSocketConnection socket)
    {
        var users = StateService.GetUsersForDevice(dto.DeviceId);
        if (users.Count == 0) //check if the users has access to device
        {
            throw new AuthenticationException("You do not have access to edit alerts from this device");
        }

        var alert = _alertService.EditAlert(dto.AlertId, dto.IsRead);

        _serverResponse.SendAlertToClient(alert);

        return Task.CompletedTask;
    }
}