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

public class ClientWantsToGetHumidityReadingsDto : BaseDto
{
    [Required(ErrorMessage = "Device Id is required")]
    public int DeviceId { get; set; }
    
    public DateTime? StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetHumidityReadings : BaseEventHandler<ClientWantsToGetHumidityReadingsDto>
{
    private readonly DeviceReadingsService _deviceReadingsService;
    public ClientWantsToGetHumidityReadings(DeviceReadingsService deviceReadingsService)
    {
        _deviceReadingsService = deviceReadingsService;
    }

    public override Task Handle(ClientWantsToGetHumidityReadingsDto dto, IWebSocketConnection socket)
    {
        
        var guid = socket.ConnectionInfo.Id;

        if (!StateService.UserHasDevice(guid, dto.DeviceId))
        {
            throw new AuthenticationException("Only the owner of device #"+dto.DeviceId+" has access to this information");
        }
        
        var readings =
            _deviceReadingsService.GetHumidityReadingsFromDevice(dto.DeviceId, dto.StartTime, dto.EndTime);
        
        socket.SendDto(new ServerSendsHumidityReadings()
        {
            DeviceId = dto.DeviceId,
            Readings = readings
        });

        return Task.CompletedTask;
    }
}