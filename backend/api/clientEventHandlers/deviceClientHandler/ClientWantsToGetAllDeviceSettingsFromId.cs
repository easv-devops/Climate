using System.ComponentModel.DataAnnotations;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsGetDeviceSettingsDto : BaseDto
{ 
    [Required(ErrorMessage = "Device Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Device Id is not a valid number")]
    public int Id { get; set; }
}

public class ClientWantsToGetAllDeviceSettingsFromId : BaseEventHandler<ClientWantsGetDeviceSettingsDto>
{
    private readonly DeviceSettingsService _deviceSettingsService;

    public ClientWantsToGetAllDeviceSettingsFromId(DeviceSettingsService deviceSettingsService)
    {
        _deviceSettingsService = deviceSettingsService;
    }
    
    public override Task Handle(ClientWantsGetDeviceSettingsDto dto, IWebSocketConnection socket)
    {
        if (!StateService.UserHasDevice(socket.ConnectionInfo.Id, dto.Id))
        {
            throw new UnauthorizedAccessException("only the owner of given device can access this information");
        }
        
        var rangeSettings = _deviceSettingsService.GetRangeSettings(dto.Id);
        
        socket.SendDto(new ServerSendsDeviceRangeSettings
        {
            Settings = rangeSettings
        });

        var deviceSettings = _deviceSettingsService.GetDeviceSettingsFromId(dto.Id);
        
        socket.SendDto(new ServerSendsDeviceSettings
        {
            Settings = deviceSettings
        });

        return Task.CompletedTask;
    }
}
