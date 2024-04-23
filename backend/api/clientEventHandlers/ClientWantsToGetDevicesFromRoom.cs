using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetDevicesFromRoomDto : BaseDto
{
    [Required(ErrorMessage = "Room Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Room Id is not a valid number")]
    public int RoomId { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetDevicesFromRoom : BaseEventHandler<ClientWantsToGetDevicesFromRoomDto>
{
    private readonly DeviceService _deviceService;

    public ClientWantsToGetDevicesFromRoom(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    public override Task Handle(ClientWantsToGetDevicesFromRoomDto fromRoomDto, IWebSocketConnection socket)
    {
        var devices = _deviceService.GetDevicesByRoomId(fromRoomDto.RoomId);
        
        socket.SendDto(new ServerSendsDevicesFromRoom
        {
            RoomId = fromRoomDto.RoomId,
            Devices = devices
        });
        
        return Task.CompletedTask;
    }
}