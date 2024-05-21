using System.ComponentModel.DataAnnotations;

namespace infrastructure.Models;

public class RangeDto{
    [Required(ErrorMessage = "Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Id is not a valid number")]
    public required int Id { get; set; }
    
    [Required(ErrorMessage = "Max. Temperature is required.")]
    [Range(-10, 40, ErrorMessage = "Max. Temperature is not a valid number. Must be between -10 and 40")]
    public required int TemperatureMax { get; set; } 
    
    [Required(ErrorMessage = "Min. Temperature is required.")]
    [Range(-10, 40, ErrorMessage = "Min. Temperature is not a valid number. Must be between -10 and 40")]
    public required int TemperatureMin { get; set; } 
    
    [Required(ErrorMessage = "Max. Humidity is required.")]
    [Range(0, 100, ErrorMessage = "Max. Humidity is not a valid number. Must be between 0 and 100")]
    public required int HumidityMax { get; set; }
    
    [Required(ErrorMessage = "Min. Humidity is required.")]
    [Range(0, 100, ErrorMessage = "Min. Humidity is not a valid number. Must be between 0 and 100")]
    public required int HumidityMin { get; set; } 
    
    [Required(ErrorMessage = "Max. Particles 2.5 is required.")]
    [Range(0, 100, ErrorMessage = "Max. Particles 2.5 is not a valid number. Must be between 0 and 100")]
    public required int Particle25Max { get; set; }
    
    [Required(ErrorMessage = "Max. Particles 10 is required.")]
    [Range(0, 100, ErrorMessage = "Max. Particles 10 is not a valid number. Must be between 0 and 100")]
    public required int Particle100Max { get; set; }
}

public class SettingsDto
{
    [Required(ErrorMessage = "Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Id is not a valid number")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "BMP280 Reading Interval is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "BMP280 Reading Interval is not a valid number")]
    public int BMP280ReadingInterval { get; set; }
    
    [Required(ErrorMessage = "PMS Reading Interval is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "PMS Reading Interval is not a valid number")]
    public int PMSReadingInterval { get; set; }
    
    [Required(ErrorMessage = "Update Interval is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Update Interval is not a valid number")]
    public int UpdateInterval { get; set; }
}
