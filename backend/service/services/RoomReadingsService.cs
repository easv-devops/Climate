using System.Globalization;
using infrastructure.Models;

namespace service.services;

public class RoomReadingsService
{
    private readonly DeviceService _deviceService;
    private readonly DeviceReadingsService _deviceReadingsService;

    public RoomReadingsService(DeviceService deviceService, DeviceReadingsService deviceReadingsService)
    {
        _deviceService = deviceService;
        _deviceReadingsService = deviceReadingsService;
    }

    public IEnumerable<SensorDto> GetTemperatureReadingsFromRoom(int dtoRoom, DateTime? dtoStartTime,
        DateTime? dtoEndTime, int intervalMinutes)
    {
        var deviceIds = _deviceService.GetDeviceIdsFromRoom(dtoRoom);

        if (deviceIds == null || !deviceIds.Any())
        {
            return Enumerable.Empty<SensorDto>();
        }

        var allReadings = new List<SensorDto>();

        foreach (var deviceId in deviceIds)
        {
            var readings = _deviceReadingsService.GetTemperatureReadingsFromDevice(deviceId, dtoStartTime, dtoEndTime);
            allReadings.AddRange(readings);
        }

        return GroupAndAverageReadings(allReadings, intervalMinutes);
    }

    public IEnumerable<SensorDto> GetHumidityReadingsFromRoom(int dtoRoom, DateTime? dtoStartTime, DateTime? dtoEndTime,
        int intervalMinutes)
    {
        var deviceIds = _deviceService.GetDeviceIdsFromRoom(dtoRoom);

        if (deviceIds == null)
        {
            return Enumerable.Empty<SensorDto>();
        }

        var allReadings = new List<SensorDto>();

        foreach (var deviceId in deviceIds)
        {
            var readings = _deviceReadingsService.GetHumidityReadingsFromDevice(deviceId, dtoStartTime, dtoEndTime);
            if (readings != null)
            {
                allReadings.AddRange(readings);
            }
        }

        return GroupAndAverageReadings(allReadings, intervalMinutes);
    }

    public IEnumerable<SensorDto> GetPm25ReadingsFromRoom(int dtoRoom, DateTime? dtoStartTime, DateTime? dtoEndTime,
        int intervalMinutes)
    {
        var deviceIds = _deviceService.GetDeviceIdsFromRoom(dtoRoom);

        if (deviceIds == null || !deviceIds.Any())
        {
            return Enumerable.Empty<SensorDto>();
        }

        var allReadings = new List<SensorDto>();

        foreach (var deviceId in deviceIds)
        {
            var readings = _deviceReadingsService.GetPm25ReadingsFromDevice(deviceId, dtoStartTime, dtoEndTime);
            if (readings != null)
            {
                allReadings.AddRange(readings);
            }
        }

        return GroupAndAverageReadings(allReadings, intervalMinutes);
    }

    public IEnumerable<SensorDto> GetPm100ReadingsFromRoom(int dtoRoom, DateTime? dtoStartTime, DateTime? dtoEndTime,
        int intervalMinutes)
    {
        var deviceIds = _deviceService.GetDeviceIdsFromRoom(dtoRoom);

        if (deviceIds == null || !deviceIds.Any())
        {
            return Enumerable.Empty<SensorDto>();
        }

        var allReadings = new List<SensorDto>();

        foreach (var deviceId in deviceIds)
        {
            var readings = _deviceReadingsService.GetPm100ReadingsFromDevice(deviceId, dtoStartTime, dtoEndTime);
            if (readings != null)
            {
                allReadings.AddRange(readings);
            }
        }

        return GroupAndAverageReadings(allReadings, intervalMinutes);
    }

    //gets the average value for all values in each time interval
    private IEnumerable<SensorDto> GroupAndAverageReadings(IEnumerable<SensorDto> allReadings, int intervalMinutes)
    {
        if (!allReadings.Any())
        {
            return Enumerable.Empty<SensorDto>();
        }

        var groupedReadings = allReadings
            .GroupBy(r =>
            {
                DateTime timestamp;
                if (DateTime.TryParseExact(r.TimeStamp, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out timestamp))
                {
                    return RoundToNearestInterval(timestamp, intervalMinutes);
                }
                else
                {
                    throw new FormatException($"Invalid DateTime format: {r.TimeStamp}");
                }
            })
            .Select(g => new SensorDto
            {
                TimeStamp = g.Key.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture), // Use slashes here
                Value = g.Average(r => r.Value)
            })
            .OrderBy(r => r.TimeStamp)
            .ToList();

        return groupedReadings;
    }

    private static DateTime RoundToNearestInterval(DateTime dateTime, int intervalMinutes)
    {
        var totalMinutes = (int)Math.Round(dateTime.TimeOfDay.TotalMinutes / intervalMinutes) * intervalMinutes;
        return dateTime.Date.AddMinutes(totalMinutes);
    }
}