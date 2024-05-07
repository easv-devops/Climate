using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetPm100ReadingsDto : BaseDto
{
    [Required(ErrorMessage = "Device Id is required")]
    public int DeviceId { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetPm100Readings : BaseEventHandler<ClientWantsToGetPm100ReadingsDto>
{
    private readonly DeviceReadingsService _deviceReadingsService;
    public ClientWantsToGetPm100Readings(DeviceReadingsService deviceReadingsService)
    {
        _deviceReadingsService = deviceReadingsService;
    }

    public override Task Handle(ClientWantsToGetPm100ReadingsDto dto, IWebSocketConnection socket)
    {

        var guid = socket.ConnectionInfo.Id;

        if (!StateService.UserHasDevice(guid, dto.DeviceId))
        {
            throw new AuthenticationException("Only the owner of device #"+dto.DeviceId+" has access to this information");
        }
        
        var readings =
            _deviceReadingsService.GetPm100ReadingsFromDevice(dto.DeviceId);
        
        socket.SendDto(new ServerSendsPm100Readings()
        {
            DeviceId = dto.DeviceId,
            Pm100Readings = readings
        });

        return Task.CompletedTask;
    }
}