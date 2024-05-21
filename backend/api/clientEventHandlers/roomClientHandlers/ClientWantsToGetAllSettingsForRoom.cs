using System.ComponentModel.DataAnnotations;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers.roomClientHandlers;


public class ClientWantsGetRoomSettingsDto : BaseDto
{ 
    [Required(ErrorMessage = "Device Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Device Id is not a valid number")]
    public int Id { get; set; }
}

public class ClientWantsToGetAllSettingsForRoom : BaseEventHandler<ClientWantsGetRoomSettingsDto>
{
    private readonly DeviceSettingsService _deviceSettingsService;
    private readonly DeviceService _deviceService;

    public ClientWantsToGetAllSettingsForRoom(DeviceSettingsService deviceSettingsService, DeviceService deviceService)
    {
        _deviceService = deviceService;
        _deviceSettingsService = deviceSettingsService;
    }
    
    public override Task Handle(ClientWantsGetRoomSettingsDto dto, IWebSocketConnection socket)
    {
        if (!StateService.GetUsersForRoom(dto.Id).Any())
        {
            throw new UnauthorizedAccessException("only the owner of given device can access this information");
        }
        
        
        var deviceIds = _deviceService.GetDeviceIdsFromRoom(dto.Id);

        List<RangeDto> deviceRangeList = new List<RangeDto>();
        foreach (var deviceId in deviceIds)
        {
            deviceRangeList.Add(_deviceSettingsService.GetRangeSettings(deviceId));
        }
        
        var averageRangeSettings = new RangeDto
        {
            Id = 0, // DeviceId is not meaningful for the average
            TemperatureMax = (int)deviceRangeList.Average(setting => setting.TemperatureMax),
            TemperatureMin = (int)deviceRangeList.Average(setting => setting.TemperatureMin),
            HumidityMax = (int)deviceRangeList.Average(setting => setting.HumidityMax),
            HumidityMin = (int)deviceRangeList.Average(setting => setting.HumidityMin),
            Particle25Max = (int)deviceRangeList.Average(setting => setting.Particle25Max),
            Particle100Max = (int)deviceRangeList.Average(setting => setting.Particle100Max)
        };
        
        
        
        throw new NotImplementedException();
    }
}