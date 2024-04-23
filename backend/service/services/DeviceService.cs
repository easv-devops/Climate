using infrastructure;
using infrastructure.Models;

namespace service.services;


public class DeviceService
{

    private readonly DeviceRepository _deviceRepository;

    public DeviceService(DeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }
    public DeviceDto CreateDevice(DeviceDto deviceDto)
    {
        var device = _deviceRepository.Create(deviceDto);

        return deviceDto;
    }

    public IEnumerable<DeviceByRoomIdDto> GetDevicesByRoomId(int roomId)
    {
        return _deviceRepository.GetDevicesByRoomId(roomId);
    }
    
    public IEnumerable<DeviceFullDto> GetDevicesByUserId(int userId)
    {
        return _deviceRepository.GetDevicesByUserId(userId);
    }

    public DeviceFullDto GetDeviceById(int deviceId)
    {
        return _deviceRepository.GetDeviceById(deviceId);
    }
}