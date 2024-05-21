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
     
    public RangeDto CreateRangeSettings(RangeDto settings)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                var sql = @"
                    INSERT INTO RangeSettings (DeviceId, TemperatureMin, TemperatureMax, HumidityMin, HumidityMax, Particle25Max, Particle100Max)
                    VALUES (@DeviceId, @TemperatureMin, @TemperatureMax, @HumidityMin, @HumidityMax, @Particle25Max, @Particle100Max)";

                var affectedRows = connection.Execute(sql, new
                {
                    DeviceId = settings.Id,
                    settings.TemperatureMin,
                    settings.TemperatureMax,
                    settings.HumidityMin,
                    settings.HumidityMax,
                    settings.Particle25Max,
                    settings.Particle100Max
                });

                if (affectedRows > 0)
                {
                    return settings;
                }
                else
                {
                    throw new SqlNullValueException("No rows were inserted.");
                }
            }
            catch (Exception ex)
            {
                throw new SqlTypeException("An error occurred while creating the settings.", ex);
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
                throw new SqlTypeException("An error occurred while deleting the Device settings.", ex);
            }
        }
    }

  public RangeDto EditRangeSettings(RangeDto settings)
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
                    DeviceId = settings.Id,
                    settings.TemperatureMin,
                    settings.TemperatureMax,
                    settings.HumidityMin,
                    settings.HumidityMax,
                    settings.Particle25Max,
                    settings.Particle100Max
                });

                if (affectedRows > 0)
                {
                    return settings;
                }
            
                throw new SqlNullValueException("No rows were updated.");
                
            }
            catch (Exception ex)
            {
                throw new SqlTypeException("An error occurred while editing the settings.", ex);
            }
        }
    }

  public RangeDto GetRangeSettingsFromId(int deviceId)
  {
      using (var connection = new MySqlConnection(_connectionString))
      {
          try
          {
              connection.Open();

              var sql = "SELECT DeviceId AS Id, TemperatureMin, TemperatureMax, HumidityMin, HumidityMax, Particle25Max, Particle100Max FROM RangeSettings WHERE DeviceId = @DeviceId";

              var settings = connection.QuerySingleOrDefault<RangeDto>(sql, new { DeviceId = deviceId });

              if (settings == null)
              {
                  throw new SqlNullValueException("Device settings not found.");
              }
              return settings;
          }
          catch (Exception ex)
          {
              throw new SqlTypeException("An error occurred while retrieving the settings.", ex);
          }
      }
  }

}