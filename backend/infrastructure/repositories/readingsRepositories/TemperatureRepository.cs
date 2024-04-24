using infrastructure.Models;

namespace infrastructure.repositories.readingsRepositories;


public class TemperatureRepository
{
    
    private readonly string _connectionString;

    public TemperatureRepository(string connectionString)
    {
        _connectionString = connectionString;
        
    }
    
    public bool SaveTemperatureList(int deviceId, List<TemperatureDto> tempList)
    {
        
        throw new NotImplementedException("temperature repository, not implemented");
    }
}