using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetDeviceByIdDto : BaseDto
{
    [Required(ErrorMessage = "Device Id is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Device Id is not a valid number")]
    public int DeviceId { get; set; }
}

//TODO: Pretty sure this is no longer in use after StateService implementation. Remove it?
[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetDeviceById : BaseEventHandler<ClientWantsToGetDeviceByIdDto>
{
    private readonly DeviceService _deviceService;

    public ClientWantsToGetDeviceById(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    public override Task Handle(ClientWantsToGetDeviceByIdDto dto, IWebSocketConnection socket)
    {
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User.Id;
        var device = _deviceService.GetDeviceById(dto.DeviceId, userId);

        socket.SendDto(new ServerSendsDeviceById
        {
            Device = device
        });
        
        return Task.CompletedTask;
    }
}