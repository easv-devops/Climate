
using infrastructure;
using infrastructure.Models;

namespace service.services;

public class RoomService
{
    private readonly RoomsRepository _roomsRepository;

    public RoomService(RoomsRepository roomsRepository)
    {
        _roomsRepository = roomsRepository;
    }
    
    public List<Room> GetAllRooms(int UserId)
    {
        return _roomsRepository.GetAllRooms(UserId);
    }

    public bool DeleteRoom(Room room)
    {
        return _roomsRepository.DeleteRoom(room);
    }

    public Room CreateRoom(Room room)
    {
        return _roomsRepository.CreateRoom(room);
    }

    public Room EditRoom(Room room)
    {
        return _roomsRepository.EditRoom(room);
    }

    public Room getSpecificRoom(Room room)
    {
        return _roomsRepository.getRoomById(room);
    }
}