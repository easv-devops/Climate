using System.Net.Sockets;
using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using infrastructure.Models;

namespace api.ServerEventHandlers;

public class ServerWantsToSendDevice
{
    public bool SendDeviceToClient(DeviceWithIdDto device)
    {
        var subscribedUserList = StateService.GetConnectionsForDevice(device.Id);


        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetConnection(user);
            if (!ReferenceEquals(connection, null))
            {
                connection.Connection.SendDto(new ServerSendsDevice
                {
                    Id = device.Id,
                    DeviceName = device.DeviceName,
                    RoomId = device.RoomId
                });
            }
        }
        return true;
    }
}