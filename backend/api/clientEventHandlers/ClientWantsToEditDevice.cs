using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;


public class ClientWantsToEditDeviceDto : BaseDto
{
    public int Id { get; set; }
    
    [MaxLength(50, ErrorMessage = "Device Name is too long")]
    public string? DeviceName { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Room Id is not a valid number")]
    public int? RoomId { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToEditDevice: BaseEventHandler<ClientWantsToEditDeviceDto>
{
    
    private readonly DeviceService _deviceService;

    public ClientWantsToEditDevice(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    
    public override Task Handle(ClientWantsToEditDeviceDto dto, IWebSocketConnection socket)
    {
        //todo should check if you have access to the device 
        bool wasEdit = _deviceService.EditDevice(dto.Id, new DeviceDto
        {
            DeviceName = dto.DeviceName,
            RoomId = dto.RoomId
        });
        socket.SendDto(new ServerEditsDeviceDto
        {
            IsEdit = wasEdit
        });
        return Task.CompletedTask;
    }
}

public class ServerEditsDeviceDto: BaseDto
{
    public bool IsEdit { get; set; }
}