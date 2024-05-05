using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetPm25ReadingsDto : BaseDto
{
    [Required(ErrorMessage = "Device Id is required")]
    public int DeviceId { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetPm25Readings : BaseEventHandler<ClientWantsToGetPm25ReadingsDto>
{
    private readonly DeviceReadingsService _deviceReadingsService;
    public ClientWantsToGetPm25Readings(DeviceReadingsService deviceReadingsService)
    {
        _deviceReadingsService = deviceReadingsService;
    }

    public override Task Handle(ClientWantsToGetPm25ReadingsDto dto, IWebSocketConnection socket)
    {
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User!.Id;

        var readings =
            _deviceReadingsService.GetPm25ReadingsFromDevice(dto.DeviceId, userId);
        
        socket.SendDto(new ServerSendsPm25Readings()
        {
            DeviceId = dto.DeviceId,
            Pm25Readings = readings
        });
        
        return Task.CompletedTask;
    }
}