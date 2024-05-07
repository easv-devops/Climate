using System.Net.Sockets;
using api.helpers;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers.roomClientHandlers;

public class ClientWantsToDeleteRoomDto : BaseDto
{
    public required int RoomToDelete { get; set; }
}


public class ClientWantsToDeleteRoom: BaseEventHandler<ClientWantsToDeleteRoomDto>
{
    
    private RoomService _roomService;
    private DeviceService _deviceService;
    private readonly DeviceReadingsService _deviceReadingsService;

    public ClientWantsToDeleteRoom(RoomService roomService, DeviceService deviceService, DeviceReadingsService deviceReadingsService)
    {
        _roomService = roomService;
        _deviceService = deviceService;
        _deviceReadingsService = deviceReadingsService;
    }

    public override Task Handle(ClientWantsToDeleteRoomDto dto, IWebSocketConnection socket)
    {

        var deviceIds = _deviceService.GetDeviceIdsFromRoom(dto.RoomToDelete);

        foreach (var deviceId in deviceIds)
        {
            _deviceReadingsService.DeleteAllReadings(deviceId);
            if (!_deviceService.DeleteDevice(deviceId))
            {
                throw new Exception("could not delete device on room");
            }
        }

        bool isRoomDeleted = _roomService.DeleteRoom(dto.RoomToDelete);

        if (!isRoomDeleted)
        {
            throw new Exception("could not delete Room");
        }
        
        socket.SendDto(new ServerDeletesRoom
        {
            DeletedRoom = dto.RoomToDelete
        });
        
        return Task.CompletedTask;
    }
}

public class ServerDeletesRoom : BaseDto
{
    public int DeletedRoom { get; set; }
}