using System.Net.Sockets;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using infrastructure.Models;

namespace api.ServerEventHandlers;

public class ServerWantsToSendDevice
{
    public bool SendDeviceToClient(DeviceWithIdDto device)
    {
        var subscribedUserList = StateService.GetUsersForDevice(device.Id);

        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetClient(user);
            connection.Connection.SendDto(  new ServerSendsDeviceById
            {
                Device = device
            });
        }
        return true;
    }
}