using infrastructure;
using infrastructure.Models;

namespace service.services;

public class RoomService
{
    private readonly RoomsRepository _roomsRepository;

    public RoomService(RoomsRepository roomsRepository, DeviceRepository deviceRepository)
    {
        _roomsRepository = roomsRepository;
    }

    public IEnumerable<RoomWithId> GetAllRooms(int UserId)
    {
        return _roomsRepository.GetAllRooms(UserId);
    }
    
    public RoomWithId CreateRoom(CreateRoomDto room)
    {
        return _roomsRepository.CreateRoom(room);
    }

    public RoomWithId EditRoom(Room dto)
    {
        return _roomsRepository.EditRoom(dto);
    }
}
