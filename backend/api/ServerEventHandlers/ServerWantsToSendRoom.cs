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
     
        var subscribedUserList = StateService.GetConnectionsForRoom(room.Id);
        
        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetConnection(user);
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

