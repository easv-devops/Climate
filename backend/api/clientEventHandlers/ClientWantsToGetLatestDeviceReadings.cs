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

public class ClientWantsToGetLatestDeviceReadingsDto : BaseDto
{
    [Required(ErrorMessage = "Device Id is required")]
    public int DeviceId { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetLatestDeviceReadings : BaseEventHandler<ClientWantsToGetLatestDeviceReadingsDto>
{
    private readonly DeviceReadingsService _deviceReadingsService;
    
    public ClientWantsToGetLatestDeviceReadings(DeviceReadingsService deviceReadingsService)
    {
        _deviceReadingsService = deviceReadingsService;
    }
    public override Task Handle(ClientWantsToGetLatestDeviceReadingsDto dto, IWebSocketConnection socket)
    {
        var guid = socket.ConnectionInfo.Id;

        if (!StateService.UserHasDevice(guid, dto.DeviceId))
        {
            throw new AuthenticationException("Only the owner of device #"+dto.DeviceId+" has access to this information");
        }

        var data = _deviceReadingsService.GetLatestReadingsFromDevice(dto.DeviceId);
        
        socket.SendDto(new ServerSendsLatestDeviceReadings()
        {
            Data = data
        });

        return Task.CompletedTask;
    }
}