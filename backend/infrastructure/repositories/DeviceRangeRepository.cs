using System.Data.SqlTypes;
using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure;

public class DeviceRangeRepository
{
    private readonly string _connectionString;

    public DeviceRangeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    
    public DeviceRangeDto CreateRangeSettings(DeviceRangeDto deviceSettings)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                var sql = @"
                    INSERT INTO RangeSettings (DeviceId, TemperatureMin, TemperatureMax, HumidityMin, HumidityMax, Particle25Max, Particle100Max)
                    VALUES (@DeviceId, @TemperatureMin, @TemperatureMax, @HumidityMin, @HumidityMax, @Particle25, @Particle100)";

                var affectedRows = connection.Execute(sql, new
                {
                    deviceSettings.DeviceId,
                    deviceSettings.TemperatureMin,
                    deviceSettings.TemperatureMax,
                    deviceSettings.HumidityMin,
                    deviceSettings.HumidityMax,
                    deviceSettings.Particle25Max,
                    deviceSettings.Particle100Max
                });

                if (affectedRows > 0)
                {
                    return deviceSettings;
                }
                else
                {
                    throw new Exception("No rows were inserted.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the settings.", ex);
            }
        }
    }

    public bool DeleteRangeSettings(int deviceId)
    {

        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                var affectedRows = connection.Execute(
                    "DELETE FROM RangeSettings WHERE DeviceId = @DeviceId",
                    new { DeviceId = deviceId }
                );

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the Device settings.", ex);
            }
        }
    }

  public DeviceRangeDto EditRangeSettings(DeviceRangeDto deviceSettings)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                var sql = @"
                    UPDATE RangeSettings
                    SET TemperatureMin = @TemperatureMin,
                        TemperatureMax = @TemperatureMax,
                        HumidityMin = @HumidityMin,
                        HumidityMax = @HumidityMax,
                        Particle25Max = @Particle25Max,
                        Particle100Max = @Particle100Max
                    WHERE DeviceId = @DeviceId";

                var affectedRows = connection.Execute(sql, new
                {
                    deviceSettings.DeviceId,
                    deviceSettings.TemperatureMin,
                    deviceSettings.TemperatureMax,
                    deviceSettings.HumidityMin,
                    deviceSettings.HumidityMax,
                    deviceSettings.Particle25Max,
                    deviceSettings.Particle100Max
                });

                if (affectedRows > 0)
                {
                    return deviceSettings;
                }
            
                throw new Exception("No rows were updated.");
                
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while editing the settings.", ex);
            }
        }
    }

    public DeviceRangeDto GetRangeSettingsFromId(int deviceId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                var sql = "SELECT * FROM RangeSettings WHERE DeviceId = @DeviceId";

                var settings = connection.QuerySingleOrDefault<DeviceRangeDto>(sql, new { DeviceId = deviceId });

                if (settings == null)
                {
                    throw new Exception("Device settings not found.");
                }

                return settings;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the settings.", ex);
            }
        }
    }
}