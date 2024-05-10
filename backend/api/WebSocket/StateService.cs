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
    private static readonly Dictionary<Guid, WebSocketMetaData> Connections = new();
    
    //holds the device and the connections that wants updates on that device
    private static readonly Dictionary<int, List<Guid>> DeviceToConnections = new();
    
    //used for getting all devices a connection is subscribed to (used to empty DeviceToConnections dictionary when client disconnects)
    private static readonly Dictionary<Guid, List<int>> ConnectionToDevices = new();

    
    //holds the roomId and the connections that wants updates on that room
    private static readonly Dictionary<int, List<Guid>> RoomToConnections = new();
    
    //used for getting all rooms a connection is subscribed to (used to empty RoomToConnections dictionary when client disconnects)
    private static readonly Dictionary<Guid, List<int>> ConnectionToRooms = new();

    
    //holds the user and the connections that wants updates on that user
    private static readonly Dictionary<int, List<Guid>> UserToConnections = new();
    
    //used for getting all users a connection is subscribed to (used to empty UserToConnections dictionary when client disconnects)
    private static readonly Dictionary<Guid, List<int>> ConnectionToUsers = new();
    
    
    public static WebSocketMetaData GetConnection(Guid connectionId)
    {
        return Connections.GetValueOrDefault(connectionId) ?? throw new InvalidOperationException();
    }

    public static void AddConnection(Guid connectionId, IWebSocketConnection connection)
    {
        Connections.TryAdd(connectionId, new WebSocketMetaData(connection));
    }
    
    /**
     * removes the connection and all current subscribes to rooms and devices
     */
    public static void RemoveConnection(Guid connectionId)
    {
        // Create a copy of connection keys to avoid modifying the collection during iteration
        var connectionKeys = Connections.Keys.ToList();

        foreach (var key in connectionKeys)
        {
            if (key != connectionId) continue;
            if (ConnectionToDevices.TryGetValue(connectionId, out var deviceList))
            {
                // Remove all devices associated with the closed connection.
                var devices = deviceList.ToList(); // Create a copy of devices to avoid modifying the collection during iteration
                foreach (var deviceId in devices)
                {
                    RemoveConnectionFromDevice(deviceId, connectionId);
                }
                // Removes the connection from connection to device list
                ConnectionToDevices.Remove(connectionId);
            }
                
            if (ConnectionToRooms.TryGetValue(connectionId, out var roomList))
            {
                var rooms = roomList.ToList();
                foreach (var roomId in rooms)
                {
                    RemoveConnectionFromRoom(roomId, connectionId);
                }
                ConnectionToRooms.Remove(connectionId);
            }
            
            if (ConnectionToUsers.TryGetValue(connectionId, out var userList))
            {
                var users = userList.ToList();
                foreach (var userId in users)
                {
                    RemoveConnectionFromUser(userId, connectionId);
                }
                ConnectionToUsers.Remove(connectionId);
            }
            
            // Remove the connection from the connections collection
            Connections.Remove(connectionId);
        }
    }
    
    public static void RemoveConnectionFromRoom(int roomId, Guid connectionId)
    {
        if (RoomToConnections.TryGetValue(roomId, out var connectionList))
        {
            connectionList.Remove(connectionId);
            if (connectionList.Count == 0)
            {
                RoomToConnections.Remove(roomId);
            }

            if (ConnectionToRooms.TryGetValue(connectionId, out var roomList))
            {
                roomList.Remove(roomId);
                if (roomList.Count == 0)
                {
                    ConnectionToRooms.Remove(connectionId);
                }
            }
        }
    }
    
    public static void AddConnectionToRoom(int roomId, Guid connectionId)
    {
        if (RoomToConnections.TryGetValue(roomId, out var connectionList))
        {
            connectionList.Add(connectionId);
        }
        else
        {
            RoomToConnections[roomId] = new List<Guid> { connectionId };
        }

        if (ConnectionToRooms.TryGetValue(connectionId, out var roomList))
        {
            roomList.Add(roomId);
        }
        else
        {
            ConnectionToRooms[connectionId] = new List<int> { roomId };
        }
    }
    
    public static List<Guid> GetConnectionsForRoom(int roomId)
    {
        return RoomToConnections.TryGetValue(roomId, out var connectionList) ? connectionList : new List<Guid>();
    }
    
    public static List<Guid> GetConnectionsForDevice(int deviceId)
    {
        // Return en tom liste hvis der ikke er nogen connections for enheden endnu
        return DeviceToConnections.TryGetValue(deviceId, out var connectionList) ? connectionList : new List<Guid>(); 
    }

    public static void AddConnectionToDevice(int deviceId, Guid connectionId)
    {
        if (DeviceToConnections.TryGetValue(deviceId, out var connectionList))
        {
            connectionList.Add(connectionId);
        }
        else
        {
            DeviceToConnections[deviceId] = new List<Guid> { connectionId };
        }
        
        // Add the device to the connection's list of subscribed devices.
        if (ConnectionToDevices.TryGetValue(connectionId, out var deviceList))
        {
            deviceList.Add(deviceId);
        }
        else
        {
            ConnectionToDevices[connectionId] = new List<int> { deviceId };
        }
    }

    public static void RemoveConnectionFromDevice(int deviceId, Guid connectionId)
    {
        if (DeviceToConnections.ContainsKey(deviceId))
        {
            DeviceToConnections[deviceId].Remove(connectionId);
            if (DeviceToConnections[deviceId].Count == 0)
            {
                DeviceToConnections.Remove(deviceId);
            }
            
            // Remove the device from the connection's list of subscribed devices.
            if (ConnectionToDevices.ContainsKey(connectionId))
            {
                ConnectionToDevices[connectionId].Remove(deviceId);
                if (ConnectionToDevices[connectionId].Count == 0)
                {
                    ConnectionToDevices.Remove(connectionId);
                }
            }
        }
    }
    
    public static bool ConnectionHasDevice(Guid connectionId, int deviceId)
    {
        if (!ConnectionToDevices.TryGetValue(connectionId, out var deviceList))
        {
            // Forbindelsen findes ikke i vores system
            return false;
        }

        // Tjek om enheden findes i forbindelsens liste over enheder
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
            connectionList.Add(connectionId);
        }
        else
        {
            UserToConnections[userId] = new List<Guid> { connectionId };
        }
        
        // Add the user to the connection's list of subscribed users.
        if (ConnectionToUsers.TryGetValue(connectionId, out var userList))
        {
            userList.Add(userId);
        }
        else
        {
            ConnectionToUsers[connectionId] = new List<int> { userId };
        }
    }
    
    public static void RemoveConnectionFromUser(int userId, Guid connectionId)
    {
        if (UserToConnections.ContainsKey(userId))
        {
            UserToConnections[userId].Remove(connectionId);
            if (UserToConnections[userId].Count == 0)
            {
                UserToConnections.Remove(userId);
            }
            
            // Remove the user from the connection's list of subscribed users.
            if (ConnectionToUsers.TryGetValue(connectionId, out List<int>? userList))
            {
                userList.Remove(userId);
                if (userList.Count == 0)
                {
                    ConnectionToUsers.Remove(connectionId);
                }
            }
        }
    }
}