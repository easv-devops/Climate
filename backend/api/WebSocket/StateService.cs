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

    
    //holds the roomId and the users that wants updates on that room
    private static readonly Dictionary<int, List<Guid>> _roomsToUser = new();
    
    //used for getting all rooms a user is subscribed to (used to empty _roomsToUser dictionary when client disconnect)
    private static readonly Dictionary<Guid, List<int>> _userToRoom = new();
    
    //holds the user and the connections that wants updates on that user
    private static readonly Dictionary<int, List<Guid>> UserToConnections = new();
    
    //used for getting the user a connection is subscribed to (used to empty UserToConnections dictionary when client disconnects)
    private static readonly Dictionary<Guid, int> ConnectionToUser = new();

    
    
    public static WebSocketMetaData GetClient(Guid clientId)
    {
        return _clients.GetValueOrDefault(clientId) ?? throw new InvalidOperationException();
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
            if (key != clientId) continue;
            if (_userToDevice.TryGetValue(clientId, out var deviceList))
            {
                // Remove all devices associated with the disconnected user.
                var devices = deviceList.ToList(); // Create a copy of devices to avoid modifying the collection during iteration
                foreach (var deviceId in devices)
                {
                    RemoveUserFromDevice(deviceId, clientId);
                }
                // Removes the user from user to device list
                _userToDevice.Remove(clientId);
            }
                
            if (_userToRoom.TryGetValue(clientId, out var roomList))
            {
                var rooms = roomList.ToList();
                foreach (var roomId in rooms)
                {
                    RemoveUserFromRoom(roomId, clientId);
                }
                _userToRoom.Remove(clientId);
            }
            
            if (ConnectionToUser.TryGetValue(clientId, out var userId))
            {
                RemoveConnectionFromUser(userId, clientId);
                
                ConnectionToUser.Remove(clientId);
            }
            
            // Remove the client from the clients collection
            _clients.Remove(clientId);
        }
    }
    
    public static void RemoveUserFromRoom(int roomId, Guid userId)
    {
        if (_roomsToUser.TryGetValue(roomId, out var userList))
        {
            userList.Remove(userId);
            if (userList.Count == 0)
            {
                _roomsToUser.Remove(roomId);
            }

            if (_userToRoom.TryGetValue(userId, out var roomList))
            {
                roomList.Remove(roomId);
                if (roomList.Count == 0)
                {
                    _userToRoom.Remove(userId);
                }
            }
        }
    }
    
    public static void AddUserToRoom(int roomId, Guid userId)
    {
        if (_roomsToUser.TryGetValue(roomId, out var value))
        {
            value.Add(userId);
        }
        else
        {
            _roomsToUser[roomId] = new List<Guid> { userId };
        }

        if (_userToRoom.TryGetValue(userId, out var value1))
        {
            value1.Add(roomId);
        }
        else
        {
            _userToRoom[userId] = new List<int> { roomId };
        }
    }
    
    public static List<Guid> GetUsersForRoom(int roomId)
    {
        return _roomsToUser.TryGetValue(roomId, out var value) ? value : new List<Guid>();
    }
    
    public static List<Guid> GetUsersForDevice(int deviceId)
    {
        if (_deviceToUser.TryGetValue(deviceId, out var userList))
        {
            return userList;
        }
        else
        {
            return new List<Guid>(); // Return en tom liste hvis der ikke er nogen brugere for enheden endnu
        }
    }

    public static void AddUserToDevice(int deviceId, Guid userId)
    {
        if (_deviceToUser.TryGetValue(deviceId, out var userList))
        {
            userList.Add(userId);
        }
        else
        {
            _deviceToUser[deviceId] = new List<Guid> { userId };
        }
        
        // Add the device to the user's list of subscribed devices.
        if (_userToDevice.TryGetValue(userId, out var deviceList))
        {
            deviceList.Add(deviceId);
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
    
    public static bool UserHasDevice(Guid userId, int deviceId)
    {
        if (!_userToDevice.TryGetValue(userId, out var deviceList))
        {
            // Brugeren findes ikke i vores system
            return false;
        }

        // Tjek om enheden findes i brugerens liste over enheder
        return deviceList.Contains(deviceId);
    }
    
    public static List<Guid> GetConnectionsForUser(int userId)
    {
        // Return en tom liste hvis der ikke er nogen connections for brugeren
        return UserToConnections.TryGetValue(userId, out var connectionList) ? connectionList : new List<Guid>(); 
    }

    public static void AddConnectionToUser(int userId, Guid connectionId)
    {
        if (UserToConnections.TryGetValue(userId, out var connectionList))
        {
            if (!connectionList.Contains(connectionId))
            {
                connectionList.Add(connectionId);
            }
        }
        else
        {
            UserToConnections[userId] = new List<Guid> { connectionId };
        }
        
        // Sets the user as the connection's subscribed user.
        ConnectionToUser[connectionId] = userId;
    }

    public static void RemoveConnectionFromUser(int userId, Guid connectionId)
    {
        if (UserToConnections.TryGetValue(userId, out List<Guid>? connectionList))
        {
            connectionList.Remove(connectionId);
            if (connectionList.Count == 0)
            {
                UserToConnections.Remove(userId);
            }
        }

        // Remove the connection from the ConnectionToUser dictionary.
        ConnectionToUser.Remove(connectionId);
    }
}