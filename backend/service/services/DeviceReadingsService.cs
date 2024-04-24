using infrastructure.Models;
using infrastructure.repositories.readingsRepositories;

namespace service.services;

public class DeviceReadingsService
{
    private HumidityRepository _humidityRepository;
    private TemperatureRepository _temperatureRepository;
    private ParticlesRepository _particlesRepository;
    
    public DeviceReadingsService(
        HumidityRepository humidityRepository,
        TemperatureRepository temperatureRepository,
        ParticlesRepository particlesRepository)
    {
        _humidityRepository = humidityRepository;
        _temperatureRepository = temperatureRepository;
        _particlesRepository = particlesRepository;
    }
    
    public void CreateReadings(DeviceReadingsDto deviceReadingsDto)
    {
        var deviceId = deviceReadingsDto.DeviceId;

        if (deviceReadingsDto.Data.Humidities.Any())
        {
            _humidityRepository.SaveHumidityList(deviceId, deviceReadingsDto.Data.Humidities);
        }
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
}