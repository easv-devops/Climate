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

    public DeviceWithIdDto CreateDevice(DeviceDto deviceDto)
    {
        var deviceWithId = _deviceRepository.Create(deviceDto);

        return deviceWithId;
    }

    public bool DeleteDevice(int Id)
    {
        return _deviceRepository.DeleteDevice(Id);
    }

    public IEnumerable<DeviceWithIdDto> GetDevicesByUserId(int userId)
    {
        return _deviceRepository.GetDevicesByUserId(userId);
    }
    
    public bool EditDevice(int dtoId, DeviceDto deviceDto)
    {
        //todo should change room id if it is present in the dto 
        return _deviceRepository.EditDevice(dtoId, deviceDto.DeviceName);
    }

    public IEnumerable<int> GetDeviceIdsFromRoom(int roomId)
    {
        return _deviceRepository.GetDeviceIdsForRoom(roomId);
    }
}