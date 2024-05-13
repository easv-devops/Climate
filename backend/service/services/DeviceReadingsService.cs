using System.Data.SqlTypes;
using System.Security.Authentication;
using infrastructure;
using infrastructure.Models;
using infrastructure.repositories.readingsRepositories;

namespace service.services;

public class DeviceReadingsService
{
    private readonly HumidityRepository _humidityRepository;
    private readonly TemperatureRepository _temperatureRepository;
    private readonly ParticlesRepository _particlesRepository;
    
    public DeviceReadingsService(
        HumidityRepository humidityRepository,
        TemperatureRepository temperatureRepository,
        ParticlesRepository particlesRepository,
        DeviceRepository deviceRepository)
    {
        _humidityRepository = humidityRepository;
        _temperatureRepository = temperatureRepository;
        _particlesRepository = particlesRepository;
    }
    
    public void CreateReadings(DeviceData deviceReadingsDto)
    {
        var deviceId = deviceReadingsDto.DeviceId;

        if (deviceReadingsDto.Data.Humidities.Any())
            _humidityRepository.SaveHumidityList(deviceId, deviceReadingsDto.Data.Humidities);
        else
            throw new NullReferenceException("there is no humidity readings in dataset");

        if (deviceReadingsDto.Data.Temperatures.Any())
            _temperatureRepository.SaveTemperatureList(deviceId, deviceReadingsDto.Data.Temperatures);
        else
            throw new NullReferenceException("there is no Temperature readings in dataset");

        if (deviceReadingsDto.Data.Particles25.Any())
            _particlesRepository.SaveParticle25List(deviceId, deviceReadingsDto.Data.Particles25);
        else
            throw new NullReferenceException("there is no 2.5 particles readings in dataset");
        
        if (deviceReadingsDto.Data.Particles100.Any())
            _particlesRepository.SaveParticle100List(deviceId, deviceReadingsDto.Data.Particles100);
        else
            throw new NullReferenceException("there is no 10 particles readings in dataset");
    }

    public bool DeleteAllReadings(int deviceId)
    {
        var wasHumidityDeleted = _humidityRepository.DeleteHumidityReadings(deviceId);
        if (!wasHumidityDeleted)
            throw new SqlTypeException("Failed to delete humidity readings");

        var wasTemperatureDeleted = _temperatureRepository.DeleteTemperatureReadings(deviceId);
        if (!wasTemperatureDeleted)
            throw new SqlTypeException("Failed to delete temperature readings");

        var wasParticle25Deleted = _particlesRepository.DeleteParticle25(deviceId);
        if (!wasParticle25Deleted)
            throw new SqlTypeException("Failed to delete particle 2.5 readings");

        var wasParticle100Deleted = _particlesRepository.DeleteParticle100(deviceId);
        if (!wasParticle100Deleted)
            throw new SqlTypeException("Failed to delete particle 10.0 readings");
        
        return true;
    }

    public IEnumerable<SensorDto> GetTemperatureReadingsFromDevice(int deviceId, DateTime? startTime, DateTime? endTime)
    {
        return _temperatureRepository.GetTemperatureReadingsFromDevice( deviceId,  startTime, endTime);
    }
    
    public IEnumerable<SensorDto> GetHumidityReadingsFromDevice(int deviceId, DateTime? startTime, DateTime? endTime)
    {
        return _humidityRepository.GetHumidityReadingsFromDevice(deviceId, startTime, endTime);
    }
    
    public IEnumerable<SensorDto> GetPm25ReadingsFromDevice(int deviceId, DateTime? startTime, DateTime? endTime)
    {
        return _particlesRepository.GetPm25ReadingsFromDevice(deviceId, startTime, endTime);
    }
    
    public IEnumerable<SensorDto> GetPm100ReadingsFromDevice(int deviceId, DateTime? startTime, DateTime? endTime)
    {
        return _particlesRepository.GetPm100ReadingsFromDevice(deviceId, startTime, endTime);
    }
}