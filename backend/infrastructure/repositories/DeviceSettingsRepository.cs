using infrastructure.Models;

namespace infrastructure;

public class DeviceSettingsRepository
{
    
    
    private readonly string _connectionString;

    public DeviceSettingsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DeviceSettingsFullDto Create(DeviceSettingsFullDto deviceSettings)
    {
        throw new NotImplementedException("not implemented repo method");
    }

    public bool DeleteSettings(int deviceId)
    {
        throw new NotImplementedException("not implemented repo method");
    }

    public DeviceSettingsFullDto EditSettings(DeviceSettingsFullDto deviceSettings)
    {
        throw new NotImplementedException("not implemented repo method");
    }

    public DeviceSettingsFullDto GetDeviceSettingsFromId(int deviceId)
    {
        throw new NotImplementedException("not implemented repo method");
    }
    
    
}