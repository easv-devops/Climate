using infrastructure;
using infrastructure.Models;

namespace service.services;

//todo call create when creating a device (either with some standard settings or user settings??)
//todo make endpoint for getting settings for a device, should be called on the device page.
//todo create endpoint for edit Device settings (should be called when device settings are changed in frontend)

//todo create a getSettings for room service method, that gets all device settings for the room and makes an average,
//todo create a EditRoomSettings service method that loops over devices and edits with new information

//todo create editRoomSettings Endpoint

//todo get settings for device and save in state service
//todo when a getting a new message from mqtt, check the value against settings in stateservice

public class DeviceSettingsService
{

    private readonly DeviceSettingsRepository _deviceSettingsRepository;

    public DeviceSettingsService(DeviceSettingsRepository deviceSettingsRepository)
    {
        _deviceSettingsRepository = deviceSettingsRepository;
    }

    public DeviceSettingsFullDto CreateDeviceSetting(DeviceSettingsFullDto deviceSettings)
    {
        return _deviceSettingsRepository.Create(deviceSettings);
    }

    public bool DeleteDeviceSettings(int deviceId)
    {
        return _deviceSettingsRepository.DeleteSettings(deviceId);
    }

    public DeviceSettingsFullDto EditDeviceSettings(DeviceSettingsFullDto deviceSettings)
    {
        return _deviceSettingsRepository.EditSettings(deviceSettings);
    }

    public DeviceSettingsFullDto GetDeviceSettings(int deviceId)
    {
        return _deviceSettingsRepository.GetDeviceSettingsFromId(deviceId);
    }
}