namespace infrastructure.Models;

public class DeviceSettingsModel
{
    
}

public class DeviceRangeDto{
    public required int DeviceId { get; set; }
    public required int TemperatureMax { get; set; } 
    public required int TemperatureMin { get; set; } 
    public required int HumidityMax { get; set; } 
    public required int HumidityMin { get; set; } 
    public required int Particle25Max { get; set; } 
    public required int Particle100Max { get; set; }
}

public class DeviceSettingsDto
{
    public int DeviceId { get; set; }
    public int BMP280ReadingInterval { get; set; }
    public int PMSReadingInterval { get; set; }
    public int UpdateInterval { get; set; }
}
