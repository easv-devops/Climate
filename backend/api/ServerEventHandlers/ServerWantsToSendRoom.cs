using api.clientEventHandlers;
using api.helpers;
using api.WebSocket;
using infrastructure.Models;
using lib;

namespace api.ServerEventHandlers;



public class ServerWantsToSendRoom
{
    public bool SendRoomToClient(RoomWithId room)
    {
        var subscribedUserList = StateService.GetUsersForRoom(room.Id);
        
        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetClient(user);
            if (!ReferenceEquals(connection, null))
            {
                connection.Connection.SendDto(new ServerSendsRoom
                {
                    Room = room
                });
            }
        }
        return true;
    }
    
}

public class ServerSendsRoom : BaseDto
{
    public required RoomWithId Room { get; set; }
}