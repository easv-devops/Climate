using System.Security.Authentication;
using api.WebSocket;
using Fleck;
using lib;

namespace api.ClientEventFilters;

public class RequireAuthenticationAttribute : BaseEventFilter
{
    public override Task Handle<T>(IWebSocketConnection socket, T dto)
    {
        if (!StateService.GetClient(socket.ConnectionInfo.Id).IsAuthenticated)
            throw new AuthenticationException("Client is not authenticated!");
        return Task.CompletedTask;
    }
}