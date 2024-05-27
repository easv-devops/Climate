using infrastructure;
using infrastructure.Models;
using infrastructure.repositories;

namespace service.services;

public class AlertService
{
    private readonly AlertRepository _alertRepository;
    private readonly DeviceRangeRepository _deviceRangeRepository;

    public AlertService(AlertRepository alertRepository, DeviceRangeRepository deviceRangeRepository)
    {
        _alertRepository = alertRepository;
        _deviceRangeRepository = deviceRangeRepository;
    }

    public List<AlertDto> ScreenReadings(DeviceData dto)
    {
        var ranges = _deviceRangeRepository.GetRangeSettingsFromId(dto.DeviceId);
        var alerts = new List<AlertDto>();

        alerts.AddRange(ValidateAndCreateAlerts(dto.DeviceId, dto.Data.Humidities, ranges.HumidityMin,
            ranges.HumidityMax, "Humidity"));
        alerts.AddRange(ValidateAndCreateAlerts(dto.DeviceId, dto.Data.Temperatures, ranges.TemperatureMin,
            ranges.TemperatureMax, "Temperature"));
        alerts.AddRange(ValidateAndCreateAlerts(dto.DeviceId, dto.Data.Particles25, 0, ranges.Particle25Max,
            "PM 2.5"));
        alerts.AddRange(ValidateAndCreateAlerts(dto.DeviceId, dto.Data.Particles100, 0, ranges.Particle100Max,
            "PM 10"));

        return alerts;
    }

    private IEnumerable<AlertDto> ValidateAndCreateAlerts(int deviceId, List<SensorDto> readings, double min,
        double max, string readingType)
    {
        if (readings == null || readings.Count == 0)
            throw new NullReferenceException($"there is no {readingType} readings in dataset");

        var alerts = new List<AlertDto>();

        foreach (var reading in readings)
        {
            if (reading.Value < min)
            {
                var alert = CreateAlertMessage(deviceId, reading, readingType, "too low", min, max);
                alerts.Add(alert);
            }

            if (reading.Value > max)
            {
                var alert = CreateAlertMessage(deviceId, reading, readingType, "too high", min, max);
                alerts.Add(alert);
            }
        }

        return alerts;
    }

    private AlertDto CreateAlertMessage(int deviceId, SensorDto reading, string readingType, string condition,
        double? min, double? max)
    {
        string description = $"{readingType} was {condition}! " +
                             $"Reading showed {reading.Value} (Your range: {min} - {max})";
        var alertDto = new CreateAlertDto
        {
            DeviceId = deviceId,
            Timestamp = reading.TimeStamp,
            Description = description,
            IsRead = false
        };

        return CreateAlert(alertDto);
    }

    private AlertDto CreateAlert(CreateAlertDto dto)
    {
        return _alertRepository.CreateAlert(dto);
    }

    public IEnumerable<AlertDto> GetAlertsForUser(int userId, bool isRead)
    {
        return _alertRepository.GetAlertsForUser(userId, isRead);
    }

    public AlertDto EditAlert(int alertId, bool isRead)
    {
        return _alertRepository.EditAlert(alertId, isRead);
    }

    public bool DeleteAlerts(int deviceId)
    {
        return _alertRepository.DeleteAlerts(deviceId);
    }
}