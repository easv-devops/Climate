using System.ComponentModel.DataAnnotations;
using api.helpers;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;


public class ClientWantsToGetDeviceIdsForRoomDto : BaseDto
{
    [Required(ErrorMessage = "Room Id is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Room Id is not a valid number")]
    public int RoomId { get; set; }
}


public class ClientWantsToGetDeviceIdsForRoom: BaseEventHandler<ClientWantsToGetDeviceIdsForRoomDto>
{
    private readonly DeviceService _deviceService;

    public ClientWantsToGetDeviceIdsForRoom(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    
    public override Task Handle(ClientWantsToGetDeviceIdsForRoomDto dto, IWebSocketConnection socket)
    {

        var idList = _deviceService.GetDeviceIdsFromRoom(dto.RoomId);
        
        socket.SendDto(new ServerSendsDeviceIdListForRoom
        {
            DeviceIds = idList,
            RoomId = dto.RoomId
        });
        return Task.CompletedTask;
    }
}

public class ServerSendsDeviceIdListForRoom : BaseDto
{
    public IEnumerable<int> DeviceIds { get; set; }
    public int? RoomId { get; set; }
}