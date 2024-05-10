using api.clientEventHandlers;
using api.helpers;
using api.WebSocket;
using infrastructure.Models;
using lib;

namespace api.ServerEventHandlers;

public class ServerWantsToSendUser
{
    public bool SendUserToClient(FullUserDto userDto)
    {
        var subscribedConnectionsList = StateService.GetConnectionsForUser(userDto.Id);

        foreach (var conn in subscribedConnectionsList)
        {
            var connection = StateService.GetConnection(conn);
            if (!ReferenceEquals(connection, null))
            {
                connection.Connection.SendDto(new ServerSendsUser()
                {
                    FullUserDto = new FullUserDto()
                    {
                        Id = userDto.Id,
                        Email = userDto.Email,
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        CountryCode = userDto.CountryCode,
                        Number = userDto.Number
                    }
                });
            }
        }
        
        return true;
    }
}

public class ServerSendsUser : BaseDto
{
    public required FullUserDto FullUserDto { get; set; }
}