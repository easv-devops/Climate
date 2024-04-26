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
    
    
}