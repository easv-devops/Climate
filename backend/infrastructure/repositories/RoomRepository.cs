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

    public List<Room> GetAllRooms(int UserId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
                    SELECT * From climate.Room WHERE UserId = @UserId;
                ";
            List<Room> rooms = connection.Query<Room>(query, new {UserId}).ToList();
            return rooms;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool DeleteRoom(Room room)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
                DELETE FROM climate.Room WHERE Id = @id AND UserId = @UserId;
                ";
            Room rooms = connection.ExecuteScalar<Room>(query, new { id = room.Id, UserId = room.UserId });

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Room CreateRoom(Room room)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            Console.WriteLine("room: " + room.RoomName);
            connection.Open();
            string query = @"
                INSERT INTO climate.Room(UserId, RoomName) VALUES (@UserId, @RoomName)
                RETURNING *;
                ";
            Room rooms = connection.QueryFirstOrDefault<Room>(query, new { UserId = room.UserId, RoomName = room.RoomName });

            return rooms;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public Room EditRoom(Room room)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
                UPDATE climate.Room SET RoomName = @roomName WHERE Id = @id AND UserId = @UserId
                RETURNING *;
                ";
            Room rooms = connection.ExecuteScalar<Room>(query, new { roomName = room.RoomName, id = room.Id, UserId = room.UserId });

            return rooms;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}