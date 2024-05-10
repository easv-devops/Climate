using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using api.ClientEventFilters;
using api.helpers;
using api.ServerEventHandlers;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;


public class ClientWantsToEditDeviceDto : BaseDto
{
    [Required(ErrorMessage = "Device Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Device Id is not a valid number")]
    public required int Id { get; set; }
    
    [MaxLength(50, ErrorMessage = "Device Name is too long")]
    public required string DeviceName { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Room Id is not a valid number")]
    public required int RoomId { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToEditDevice: BaseEventHandler<ClientWantsToEditDeviceDto>
{
    
    private readonly DeviceService _deviceService;
    private readonly ServerWantsToSendDevice _serverWantsToSendDevice;

    public ClientWantsToEditDevice(DeviceService deviceService, ServerWantsToSendDevice serverWantsToSendDevice)
    {
        _deviceService = deviceService;
        _serverWantsToSendDevice = serverWantsToSendDevice;
    }

    
    public override Task Handle(ClientWantsToEditDeviceDto dto, IWebSocketConnection socket)
    {
        var users = StateService.GetConnectionsForDevice(dto.Id);
        if (!StateService.GetConnectionsForDevice(dto.Id).Any())
        {
            throw new AuthenticationException("You do not have access to edit this device");
        }
        //todo should check if you have access to the device 
        bool wasEdit = _deviceService.EditDevice(new DeviceWithIdDto
        {
            Id = dto.Id,
            DeviceName = dto.DeviceName,
            RoomId = dto.RoomId
        });
        socket.SendDto(new ServerEditsDeviceDto
        {
            IsEdit = wasEdit
        });
        
        //trigger server to client update, that updates device on all subscribing clients
        _serverWantsToSendDevice.SendDeviceToClient(new DeviceWithIdDto
        {
            Id = dto.Id,
            DeviceName = dto.DeviceName,
            RoomId = dto.RoomId
        });
        return Task.CompletedTask;
    }
}

public class ServerEditsDeviceDto: BaseDto
{
    public bool IsEdit { get; set; }
}