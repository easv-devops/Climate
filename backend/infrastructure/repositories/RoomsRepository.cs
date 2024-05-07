using System.Data.SqlTypes;
using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure;

public class RoomsRepository
{
    private readonly string _connectionString;

    public RoomsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<RoomWithId> GetAllRooms(int UserId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
                    SELECT * From climate.Room WHERE UserId = @UserId;
                ";
            return connection.Query<RoomWithId>(query, new { UserId = UserId });
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to get all rooms", e);
        }
    }
    
    public RoomWithId CreateRoom(CreateRoomDto room)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
            INSERT INTO climate.Room(UserId, RoomName) VALUES (@UserId, @RoomName)
            RETURNING Id, RoomName;
            ";
        
            return connection.QueryFirst<RoomWithId>(query, new { UserId = room.UserId, RoomName = room.RoomName });
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to create a room", e);
        }
    }

    
    
    
    
    
    
    //todo should delete all devices in the room maybe? or what are the status..
    /**
    public bool DeleteRoom(Room room)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
                DELETE FROM climate.Room WHERE Id = @id AND UserId = @UserId;
                ";
            connection.Execute(query, new { id = room.Id, UserId = room.UserId });
            return true;
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to delete the specific room", e);
        }
    }
    */

    



    /**
    public Room EditRoom(Room room)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
                UPDATE climate.Room 
                SET RoomName = @roomName 
                WHERE Id = @id 
                AND UserId = @UserId;
            ";
            Room rooms = connection.ExecuteScalar<Room>(query,
                new { roomName = room.RoomName, id = room.Id, UserId = room.UserId });

            return rooms;
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to update the given room", e);
        }
    }

    public Room getRoomById(Room room)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
                SELECT * FROM climate.Room WHERE Id = @id AND UserId = @UserId
                ";
            
            Room specificRoom = connection.QueryFirstOrDefault<Room>(query, new { id = room.Id, UserId = room.UserId });
            return specificRoom;
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to get the specific room", e);
        }
    }
    */
}
