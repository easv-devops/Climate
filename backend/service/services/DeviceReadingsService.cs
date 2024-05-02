using System.Data.SqlTypes;
using System.Security.Authentication;
using infrastructure;
using infrastructure.Models;
using infrastructure.repositories.readingsRepositories;

namespace service.services;

public class DeviceReadingsService
{
    private HumidityRepository _humidityRepository;
    private TemperatureRepository _temperatureRepository;
    private ParticlesRepository _particlesRepository;
    private DeviceRepository _deviceRepository;
    
    public DeviceReadingsService(
        HumidityRepository humidityRepository,
        TemperatureRepository temperatureRepository,
        ParticlesRepository particlesRepository,
        DeviceRepository deviceRepository)
    {
        _humidityRepository = humidityRepository;
        _temperatureRepository = temperatureRepository;
        _particlesRepository = particlesRepository;
        _deviceRepository = deviceRepository;
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

    public IEnumerable<SensorDto> GetTemperatureReadingsFromDevice(int deviceId, int userId)
    {
        if(!_deviceRepository.IsItUsersDevice(deviceId, userId))
            throw new AuthenticationException
                ("Only the owner of device #"+deviceId+" has access to this information");
        return _temperatureRepository.GetTemperatureReadingsFromDevice(deviceId);
    }
}