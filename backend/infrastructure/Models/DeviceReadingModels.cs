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

public class LatestData
{
    public int Id { get; set; } // DeviceId (og RoomId hvis den skal bruges til at sende RoomAvg)
    public LatestReadingsDto? Data { get; set; }
}

public class LatestReadingsDto
{
    public SensorDto? Temperature { get; set; }
    public SensorDto? Humidity { get; set; }
    public SensorDto? Particle25 { get; set; }
    public SensorDto? Particle100 { get; set; }
}
