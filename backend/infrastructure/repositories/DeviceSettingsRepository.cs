using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure.repositories;

public class DeviceSettingsRepository
{
    private readonly string _connectionString;

    public DeviceSettingsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Create
    public DeviceSettingsDto Create(DeviceSettingsDto deviceSettings)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                var sql = @"
                    INSERT INTO DeviceSettings (DeviceId, BMP280ReadingInterval, PMSReadingInterval, UpdateInterval)
                    VALUES (@DeviceId, @BMP280ReadingInterval, @PMSReadingInterval, @UpdateInterval)";

                var affectedRows = connection.Execute(sql, new
                {
                    deviceSettings.DeviceId,
                    deviceSettings.BMP280ReadingInterval,
                    deviceSettings.PMSReadingInterval,
                    deviceSettings.UpdateInterval
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
                // Handle exceptions, maybe log them
                throw new Exception("An error occurred while creating the device settings.", ex);
            }
        }
    }

    // Read
    public DeviceSettingsDto GetDeviceSettingsFromId(int deviceId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                var sql = "SELECT * FROM DeviceSettings WHERE DeviceId = @DeviceId";

                var settings = connection.QuerySingleOrDefault<DeviceSettingsDto>(sql, new { DeviceId = deviceId });

                if (settings == null)
                {
                    throw new Exception("Device settings not found.");
                }

                return settings;
            }
            catch (Exception ex)
            {
                // Handle exceptions, maybe log them
                throw new Exception("An error occurred while retrieving the device settings.", ex);
            }
        }
    }

    // Update
    public DeviceSettingsDto EditSettings(DeviceSettingsDto deviceSettings)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                var sql = @"
                    UPDATE DeviceSettings
                    SET BMP280ReadingInterval = @BMP280ReadingInterval,
                        PMSReadingInterval = @PMSReadingInterval,
                        UpdateInterval = @UpdateInterval
                    WHERE DeviceId = @DeviceId";

                var affectedRows = connection.Execute(sql, new
                {
                    deviceSettings.DeviceId,
                    deviceSettings.BMP280ReadingInterval,
                    deviceSettings.PMSReadingInterval,
                    deviceSettings.UpdateInterval
                });

                if (affectedRows > 0)
                {
                    return deviceSettings;
                }
                else
                {
                    throw new Exception("No rows were updated.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, maybe log them
                throw new Exception("An error occurred while editing the device settings.", ex);
            }
        }
    }

    // Delete
    public bool DeleteSettings(int deviceId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                var sql = "DELETE FROM DeviceSettings WHERE DeviceId = @DeviceId";

                var affectedRows = connection.Execute(sql, new { DeviceId = deviceId });

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                // Handle exceptions, maybe log them
                throw new Exception("An error occurred while deleting the device settings.", ex);
            }
        }
    }
}
