namespace infrastructure.Models;

public class DeviceReadingModels
{
    
}

public class DeviceReadingsDto
{
    public int DeviceId { get; set; }
    public object Data { get; set; }
}


public class Data
{

    public List<TempReading> TempData { get; set; }
    public List<HumiReading> HumiData { get; set; }
    public List<Part2_5Reading> Part2_5Data { get; set; }
    public List<Part10Reading> Part10Data { get; set; }
}

public class TempReading
{
    public Double Temp { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class HumiReading
{
    public Double Humi { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class Part2_5Reading
{
    public int Part2_5 { get; set; }
    public DateTime TimeStamp { get; set; }
}

public class Part10Reading
{
    public int Part10 { get; set; }
    public DateTime TimeStamp { get; set; }
}