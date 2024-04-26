using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;



public class ClientWantsToDeleteDeviceDto : BaseDto
{
    public int Id { get; set; }
}


[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToDeleteDevice : BaseEventHandler<ClientWantsToDeleteDeviceDto>
{
    private readonly DeviceService _deviceService;

    public ClientWantsToDeleteDevice(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    
    public override Task Handle(ClientWantsToDeleteDeviceDto dto, IWebSocketConnection socket)
    {
       _deviceService.DeleteDevice(dto.Id);
       socket.SendDto(new ServerDeletesDevice { IsDeleted = true});
       return Task.CompletedTask;
    }
    
}

public class ServerDeletesDevice: BaseDto
{
    //[Required(ErrorMessage = "No status for reset of password")]
    public bool IsDeleted { get; set; }
    
}