namespace infrastructure.Models;


public class DeviceReadingsDto
{
    public int DeviceId { get; set; }
    public object Data { get; set; }
}

public class Data
{

    public List<TemperatureData> Temperatures { get; set; }
    public List<HumiditiesData> Humidities { get; set; }
    public List<Particles25Data> Particles25 { get; set; }
    public List<Particles100Data> Particles100 { get; set; }
}

public class TemperatureData
{
    public Double Temperature { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class HumiditiesData
{
    public Double Humidity { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class Particles25Data
{
    public int Particle25 { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class Particles100Data
{
    public int Particle100 { get; set; }
    public DateTime TimeStamp { get; set; }
}