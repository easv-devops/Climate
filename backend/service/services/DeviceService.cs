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
    
    
}