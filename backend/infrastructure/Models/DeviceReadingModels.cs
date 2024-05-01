namespace infrastructure.Models;


public class SensorDto
{
    public double Value { get; set; }
    public string TimeStamp { get; set; }
}

public class DeviceReadingsDto
{
    public List<SensorDto> Temperatures { get; set; }
    public List<SensorDto> Humidities { get; set; }
    public List<SensorDto> Particles25 { get; set; }
    public List<SensorDto> Particles100 { get; set; }
}

public class DeviceData
{
    public int DeviceId { get; set; }
    public DeviceReadingsDto Data { get; set; }
}
