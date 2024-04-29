using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToDeleteDeviceDto : BaseDto
{ 
    [Required(ErrorMessage = "Device Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Device Id is not a valid number")]
    public int Id { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToDeleteDevice : BaseEventHandler<ClientWantsToDeleteDeviceDto>
{
    private readonly DeviceService _deviceService;
    private readonly DeviceReadingsService _deviceReadingsService;

    public ClientWantsToDeleteDevice(DeviceService deviceService, DeviceReadingsService deviceReadingsService)
    {
        _deviceService = deviceService;
        _deviceReadingsService = deviceReadingsService;
    }
    
    
    public override Task Handle(ClientWantsToDeleteDeviceDto dto, IWebSocketConnection socket)
    {
        //checks if the user has permission before deleting
        if (!_deviceService.IsItUsersDevice(dto.Id, StateService.GetClient(socket.ConnectionInfo.Id).User.Id))
        {
            throw new AccessViolationException("Ypu do not have permission to delete this device");
        }
        //removes the device from stateService
        StateService.RemoveUserFromDevice(dto.Id, socket.ConnectionInfo.Id);
        
        //return the is deleted bool
        socket.SendDto(new ServerSendsDeviceDeletionStatus
        {
            IsDeleted = _deviceService.DeleteDevice(dto.Id),
            Id = dto.Id
        });
        
        return Task.CompletedTask;
    }
}
