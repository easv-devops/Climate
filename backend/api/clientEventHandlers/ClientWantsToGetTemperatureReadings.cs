using System.ComponentModel.DataAnnotations;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetTemperatureReadingsDto : BaseDto
{
    [Required(ErrorMessage = "Device Id is required")]
    public int DeviceId { get; set; }
}

public class ClientWantsToGetTemperatureReadings : BaseEventHandler<ClientWantsToGetTemperatureReadingsDto>
{
    private readonly DeviceReadingsService _deviceReadingsService;
    public ClientWantsToGetTemperatureReadings(DeviceReadingsService deviceReadingsService)
    {
        _deviceReadingsService = deviceReadingsService;
    }

    public override Task Handle(ClientWantsToGetTemperatureReadingsDto dto, IWebSocketConnection socket)
    {
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User!.Id;

        var readings =
            _deviceReadingsService.GetTemperatureReadingsFromDevice(dto.DeviceId, userId);
        
        socket.SendDto(new ServerSendsTemperatureReadings
        {
            TemperatureReadings = readings
        });

        return Task.CompletedTask;
    }
}