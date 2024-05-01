using infrastructure;
using infrastructure.Models;

namespace service.services;

public class RoomService
{
    private readonly RoomsRepository _roomsRepository;
    public List<Room> GetAllRooms()
    {
        return _roomsRepository.GetAllRooms();
    }

    public bool DeleteRoom(Room room)
    {
        return _roomsRepository.DeleteRoom(room);
    }

    public Room CreateRoom(Room room)
    {
        return _roomsRepository.CreateRoom();
    }

    public Room EditRoom(Room room)
    {
        return _roomsRepository.EditRoom(room);
    }
}