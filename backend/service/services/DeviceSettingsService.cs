﻿using infrastructure;
using infrastructure.Models;
using infrastructure.repositories;

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