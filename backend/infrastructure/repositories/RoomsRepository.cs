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


    /**
     * 
     */
    public bool DeleteRoom(Room room)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
                DELETE FROM climate.Room WHERE Id = @id;
                ";
            return connection.Execute(query, new { Id = room.Id }) == 1;
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to delete the specific room", e);
        }
    }

    
    
 
    public RoomWithId EditRoom(Room dto)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
            UPDATE climate.Room 
            SET RoomName = @roomName 
            WHERE Id = @id;
            SELECT LAST_INSERT_ID() AS Id, @roomName AS RoomName;
        ";

            // Udfører opdateringsforespørgslen og henter det opdaterede rum
            RoomWithId updatedRoom = connection.QueryFirst<RoomWithId>(query, new { roomName = dto.RoomName, id = dto.Id });

            return updatedRoom;
        }
        catch (Exception e)
        {
            throw new SqlTypeException("Failed to update the given room", e);
        }
    }

}
