namespace infrastructure.Models;

public class DeviceSettingsModel
{
    
}

public class DeviceSettingsFullDto{
    public required int DeviceId { get; set; }
    public required int TemperatureMax { get; set; } 
    public required int TemperatureMin { get; set; } 
    public required int HumidityMax { get; set; } 
    public required int HumidityMin { get; set; } 
    public required int Particle25 { get; set; } 
    public required int Particle100 { get; set; }
}