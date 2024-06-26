﻿using api.clientEventHandlers;
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
    private readonly UserService _userService;
    private readonly AlertService _alertService;

    public ServerWantsToInitUser(RoomService roomService, DeviceService deviceService, UserService userService, AlertService alertService)
    {
        _roomService = roomService;
        _deviceService = deviceService;
        _userService = userService;
        _alertService = alertService;
    }

    public bool InitUser(IWebSocketConnection socket)
    {
        return InitRoomMapping(socket) && InitDeviceMapping(socket) && InitUserMapping(socket) && FetchAlerts(socket);
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
        return true;
    }
    
    private bool InitUserMapping(IWebSocketConnection socket)
    {
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User.Id;

        var user = _userService.GetFullUserById(userId);
        
        socket.SendDto(new ServerSendsUser
        {
            UserDto = user
        });

        StateService.AddConnectionToUser(user.Id, socket.ConnectionInfo.Id);
        return true;
    }
    
    private bool FetchAlerts(IWebSocketConnection socket)
    {
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User.Id;
        var alerts = _alertService.GetAlertsForUser(userId, false);
        socket.SendDto(new ServerSendsAlertList()
        {
            Alerts = alerts
        });
        return true;
    }

}