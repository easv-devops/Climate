namespace infrastructure.Models;


public class DeviceReadingsDto
{
    public int DeviceId { get; set; }
    public object Data { get; set; }
}

public class Data
{
    public List<TemperatureDto> Temperatures { get; set; }
    public List<HumidityDto> Humidities { get; set; }
    public List<Particle25Dto> Particles25 { get; set; }
    public List<Particle100Dto> Particles100 { get; set; }
}

public class TemperatureDto
{
    public Double Temperature { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class HumidityDto
{
    public Double Humidity { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class Particle25Dto
{
    public int Particle25 { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class Particle100Dto
{
    public int Particle100 { get; set; }
    public DateTime TimeStamp { get; set; }
}