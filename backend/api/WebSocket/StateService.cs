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
    
    //used for getting all devices a user is subscribed to (used to empty _deviceToUser dictionary when client disconnect)
    private static readonly Dictionary<Guid, List<int>> _userToDevice = new();
    
    

    public static WebSocketMetaData GetClient(Guid clientId)
    {
        if (_clients.ContainsKey(clientId))
        {
            return _clients[clientId];
        }

        return null;
    }

    public static void AddClient(Guid clientId, IWebSocketConnection connection)
    {
        _clients.TryAdd(clientId, new WebSocketMetaData(connection));
    }
    
    /**
     * removes the client and all current subscribes to rooms and devises
     */
    public static void RemoveClient(Guid clientId)
    {
        // Create a copy of client keys to avoid modifying the collection during iteration
        var clientKeys = _clients.Keys.ToList();

        foreach (var key in clientKeys)
        {
            if (key == clientId)
            {
                if (_userToDevice.ContainsKey(clientId))
                {
                    // Remove all devices associated with the disconnected user.
                    var devices = _userToDevice[clientId].ToList(); // Create a copy of devices to avoid modifying the collection during iteration
                    foreach (var deviceId in devices)
                    {
                        RemoveUserFromDevice(deviceId, clientId);
                    }
                    // Removes the user from user to device list
                    _userToDevice.Remove(clientId);
                }
                // Remove the client from the clients collection
                _clients.Remove(clientId);
            }
        }
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
        if (_deviceToUser.ContainsKey(deviceId))
        {
            _deviceToUser[deviceId].Add(userId);
        }
        else
        {
            _deviceToUser[deviceId] = new List<Guid> { userId };
        }
        
        // Add the device to the user's list of subscribed devices.
        if (_userToDevice.ContainsKey(userId))
        {
            _userToDevice[userId].Add(deviceId);
        }
        else
        {
            _userToDevice[userId] = new List<int> { deviceId };
        }
    }

    public static void RemoveUserFromDevice(int deviceId, Guid userId)
    {
        if (_deviceToUser.ContainsKey(deviceId))
        {
            _deviceToUser[deviceId].Remove(userId);
            if (_deviceToUser[deviceId].Count == 0)
            {
                _deviceToUser.Remove(deviceId);
            }
            
            // Remove the device from the user's list of subscribed devices.
            if (_userToDevice.ContainsKey(userId))
            {
                _userToDevice[userId].Remove(deviceId);
                if (_userToDevice[userId].Count == 0)
                {
                    _userToDevice.Remove(userId);
                }
            }
        }
    }
}