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

    public List<Room> GetAllRooms()
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();
            string query = @"
                SELECT * From climate.Room;
                ";
            List<Room> rooms = connection.QueryFirstOrDefault<List<Room>>(query);
            foreach (var room in rooms)
            {
                Console.WriteLine(room.roomName);
            }
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
        throw new NotImplementedException();
    }

    public Room CreateRoom()
    {
        throw new NotImplementedException();
    }

    public Room EditRoom(Room room)
    {
        throw new NotImplementedException();
    }
}