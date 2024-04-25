using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetDevicesByRoomIdDto : BaseDto
{
    [Required(ErrorMessage = "Room Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Room Id is not a valid number")]
    public int RoomId { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetDevicesByRoomId : BaseEventHandler<ClientWantsToGetDevicesByRoomIdDto>
{
    private readonly DeviceService _deviceService;

    public ClientWantsToGetDevicesByRoomId(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }
    public override Task Handle(ClientWantsToGetDevicesByRoomIdDto dto, IWebSocketConnection socket)
    {
        //todo check that user has access to room!!
        //todo should first check if room is already loaded in state service (if loaded in state service it should just get the list from there)
        //todo if room id is not in state service, load them from repo/db
        var devices = _deviceService.GetDevicesByRoomId(dto.RoomId);
        //todo if devices are loaded from db, update relevant dictionaries in state service 
        socket.SendDto(new ServerSendsDevicesByRoomId
        {
            RoomId = dto.RoomId,
            Devices = devices
        });
        
        return Task.CompletedTask;
    }
}