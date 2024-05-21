using infrastructure;
using infrastructure.Models;
using infrastructure.repositories;

namespace service.services;

public class DeviceSettingsService
{

    private readonly DeviceRangeRepository _deviceRangeRepository;
    private readonly DeviceSettingsRepository _deviceSettingsRepository;

    public DeviceSettingsService(DeviceRangeRepository deviceRangeRepository, DeviceSettingsRepository deviceSettingsRepository)
    {
        _deviceRangeRepository = deviceRangeRepository;
        _deviceSettingsRepository = deviceSettingsRepository;
    }

    public RangeDto CreateRangeSetting(RangeDto settings)
    {
        return _deviceRangeRepository.CreateRangeSettings(settings);
    }

    public bool DeleteRangeSettings(int deviceId)
    {
        return _deviceRangeRepository.DeleteRangeSettings(deviceId);
    }

    public RangeDto EditRangeSettings(RangeDto settings)
    {
        return _deviceRangeRepository.EditRangeSettings(settings);
    }

    public RangeDto GetRangeSettings(int deviceId)
    {
        return _deviceRangeRepository.GetRangeSettingsFromId(deviceId);
    }

    public SettingsDto CreateDeviceSettings(SettingsDto settingsDto)
    {
        return _deviceSettingsRepository.Create(settingsDto);
    }

    public SettingsDto GetDeviceSettingsFromId(int deviceId)
    {
        return _deviceSettingsRepository.GetDeviceSettingsFromId(deviceId);
    }

    public SettingsDto EditSettings(SettingsDto settings)
    {
        return _deviceSettingsRepository.EditSettings(settings);
    }

    public bool DeleteSettings(int deviceId)
    {
        return _deviceSettingsRepository.DeleteSettings(deviceId);
    }
}