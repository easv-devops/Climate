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
        //todo check that user has access to room!!
        //todo should first check if room is already loaded in state service (if loaded in state service it should just get the list from there)
        //todo if room id is not in state service, load them from repo/db
        var devices = _deviceService.GetDevicesByRoomId(fromRoomDto.RoomId);
        //todo if devices are loaded from db, update relevant dictionaries in state service 
        socket.SendDto(new ServerSendsDevicesFromRoom
        {
            RoomId = fromRoomDto.RoomId,
            Devices = devices
        });
        
        return Task.CompletedTask;
    }
}