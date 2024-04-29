using Fleck;
using infrastructure.Models;

namespace api.WebSocket;

/**
 * holds the connection and relevant information for the user
 */
public class WebSocketMetaData
{
    public IWebSocketConnection Connection { get; set; }
    public bool IsAuthenticated { get; set; } = false;
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
    
    //holds the device and the users that wants updates on that device
    private static readonly Dictionary<int, List<Guid>> _deviceToUser = new();
    
    



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
    
    
    public static List<Guid> GetUsersForDevice(int deviceId)
    {
        if (_deviceToUser.ContainsKey(deviceId))
        {
            return _deviceToUser[deviceId];
        }
        else
        {
            return new List<Guid>(); // Return en tom liste hvis der ikke er nogen brugere for enheden endnu
        }
    }

    public static void AddUserToDevice(int deviceId, Guid userId)
    {
        Console.WriteLine("userid: " + userId + "  DeviceId: " + deviceId);
        if (_deviceToUser.ContainsKey(deviceId))
        {
            _deviceToUser[deviceId].Add(userId);
        }
        else
        {
            _deviceToUser[deviceId] = new List<Guid> { userId };
        }
    }

    public static void RemoveUserFromDevice(int deviceId, Guid userId)
    {
        if (_deviceToUser.ContainsKey(deviceId))
        {
            _deviceToUser[deviceId].Remove(userId);
            if (_deviceToUser[deviceId].Count == 0)
            {
                _deviceToUser.Remove(deviceId); // Hvis der ikke er flere brugere til enheden, fjern den fra dictionar
            }
        }
    }
    

}