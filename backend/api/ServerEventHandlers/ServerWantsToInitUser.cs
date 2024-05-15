using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using api.serverEventModels.roomDtos;
using api.WebSocket;
using Fleck;
using service.services;

namespace api.ServerEventHandlers;

public class ServerWantsToInitUser
{
    
    private readonly RoomService _roomService;
    private readonly DeviceService _deviceService;

    public ServerWantsToInitUser(RoomService roomService, DeviceService deviceService)
    {
        _roomService = roomService;
        _deviceService = deviceService;
    }

    public bool InitUser(IWebSocketConnection socket)
    {
        return InitRoomMapping(socket) && InitDeviceMapping(socket);
    }
    
    private bool InitRoomMapping(IWebSocketConnection socket)
    {
        var user = StateService.GetClient(socket.ConnectionInfo.Id);
        
        var roomList = _roomService.GetAllRooms(user.User!.Id);
        
        socket.SendDto(new ServerReturnsAllRooms
        {
            Rooms = roomList
        });


        foreach (var room in roomList)
        {
            StateService.AddUserToRoom(room.Id, user.Connection.ConnectionInfo.Id);
        }
        
        return true;
    }

    private bool InitDeviceMapping(IWebSocketConnection socket)
    {
        
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User.Id;
        var devices = _deviceService.GetDevicesByUserId(userId);
        
        socket.SendDto(new ServerSendsDevicesByUserId
        {
            Devices = devices
        });
        
        foreach (var device in devices)
        {
            StateService.AddUserToDevice(device.Id, socket.ConnectionInfo.Id);
        }
        return true;//todo check if mapping was succes
    }
    
}