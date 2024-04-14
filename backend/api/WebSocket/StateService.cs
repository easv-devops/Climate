using Fleck;
using infrastructure.Models;

namespace api.WebSocket;

/**
 * holds the connection and relevant information for the user
 */
public class WebSocketMetaData
{
    public IWebSocketConnection Connection { get; set; }
    public EndUser? User { get; set; }
    
    public WebSocketMetaData(IWebSocketConnection connection)
    {
        Connection = connection;
    }
}
public static class StateService
{
    //holds the connections
    private static readonly Dictionary<Guid, WebSocketMetaData> _clients = new();
    
    public static WebSocketMetaData GetClient(Guid clientId)
    {
        return _clients[clientId];
    }

    public static void AddClient(Guid clientId, IWebSocketConnection connection)
    {
        _clients.TryAdd(clientId, new WebSocketMetaData(connection));
    }
    
    /**
     * removes the client and all current subscribes to rooms and devises
     * todo Should remove all dependencies for the user 
     */
    public static void RemoveClient(Guid clientId)
    {
        _clients.Remove(clientId);
    }
}