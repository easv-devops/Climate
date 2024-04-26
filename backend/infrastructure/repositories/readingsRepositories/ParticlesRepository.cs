using infrastructure.Models;

namespace infrastructure.repositories.readingsRepositories;

public class ParticlesRepository
{
    private readonly string _connectionString;

    public ParticlesRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public bool SaveParticle25List(int deviceId, List<SensorDto> particles25List)
    {
     
        throw new NotImplementedException("temperature repository, not implemented");
    }
    
    public bool SaveParticle100List(int deviceId, List<SensorDto> particles100List)
    {
        throw new NotImplementedException("temperature repository, not implemented");
    }
}