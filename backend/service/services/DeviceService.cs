using System.Security.Authentication;
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

    public IEnumerable<DeviceByRoomIdDto> GetDevicesByRoomId(int roomId, int userId)
    {
        if(!_deviceRepository.IsItUsersRoom(roomId, userId)) 
            throw new AuthenticationException
                ("Only the owner of room #"+roomId+" has access to this information");
        
        return _deviceRepository.GetDevicesByRoomId(roomId);
    }
    
    public IEnumerable<DeviceWithIdDto> GetDevicesByUserId(int userId)
    {
        return _deviceRepository.GetDevicesByUserId(userId);
    }

    public DeviceWithIdDto GetDeviceById(int deviceId, int userId)
    {
        if(!_deviceRepository.IsItUsersDevice(deviceId, userId))
            throw new AuthenticationException
                ("Only the owner of device #"+deviceId+" has access to this information");
        
        return _deviceRepository.GetDeviceById(deviceId);
    }
}