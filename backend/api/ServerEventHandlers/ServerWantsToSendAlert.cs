using api.helpers;
using api.WebSocket;
using infrastructure.Models;
using lib;

namespace api.ServerEventHandlers;

public class ServerWantsToSendAlert
{
    public bool SendAlertToClient(AlertDto alert)
    {
        var subscribedUserList = StateService.GetUsersForDevice(alert.DeviceId);

        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetClient(user);
            if (!ReferenceEquals(connection, null))
            {
                connection.Connection.SendDto(new ServerSendsAlert
                {
                    Alert = alert
                });
            }
        }

        return true;
    }
}

public class ServerSendsAlert : BaseDto
{
    public required AlertDto Alert { get; set; }
}