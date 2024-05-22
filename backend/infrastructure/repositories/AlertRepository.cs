using System.Data.SqlTypes;
using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure.repositories;

public class AlertRepository
{
    private readonly string _connectionString;

    public AlertRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public AlertDto CreateAlert(CreateAlertDto dto)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            var sql = @"
                INSERT INTO Alert (DeviceId, Timestamp, Description, IsRead) 
                VALUES (@DeviceId, @Timestamp, @Description, @IsRead);
                SELECT LAST_INSERT_ID();";

            var alertId = connection.ExecuteScalar<int>(sql, new
            {
                dto.DeviceId, dto.Timestamp, dto.Description, dto.IsRead
            });

            return GetAlertById(alertId);
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Could not create alert for device #" + dto.DeviceId, ex);
        }
    }

    private AlertDto GetAlertById(int alertId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            var sql = @"
            SELECT
                Alert.Id,
                Alert.Timestamp,
                Alert.IsRead,
                Alert.Description,
                Device.Id AS DeviceId,
                Device.DeviceName,
                Room.RoomName
            FROM
                Alert
            JOIN
                Device ON Alert.DeviceId = Device.Id
            JOIN
                Room ON Device.RoomId = Room.Id
            JOIN
                User ON Room.UserId = User.Id
            WHERE
                Alert.Id = @AlertId;";

            return connection.QuerySingleOrDefault<AlertDto>(sql, new { AlertId = alertId }) ??
                   throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Could not get alert with ID #" + alertId, ex);
        }
    }

    public IEnumerable<AlertDto> GetAlertsForUser(int userId, bool isRead)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            var sql = @"
                SELECT
                    Alert.Id,
                    Alert.Timestamp,
                    Alert.IsRead,
                    Alert.Description,
                    Device.Id AS DeviceId,
                    Device.DeviceName,
                    Room.RoomName
                FROM
                    Alert
                JOIN
                    Device ON Alert.DeviceId = Device.Id
                JOIN
                    Room ON Device.RoomId = Room.Id
                JOIN
                    User ON Room.UserId = User.Id
                WHERE
                    User.Id = @UserId
                AND
                    Alert.IsRead = @IsRead;";

            return connection.Query<AlertDto>(sql, new { UserId = userId, IsRead = isRead });
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Could not get alerts for user #" + userId, ex);
        }
    }

    public AlertDto EditAlert(int alertId, bool isRead)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            var sql = @"
                UPDATE Alert
                SET IsRead = @IsRead
                WHERE Id = @AlertId;";

            var success = connection.Execute(sql, new { AlertId = alertId, IsRead = isRead }) == 1;

            if (!success)
            {
                throw new SqlNullValueException("Could not edit alert");
            }

            return GetAlertById(alertId);
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Could not edit alert with ID #" + alertId, ex);
        }
    }
    
    public bool DeleteAlerts(int deviceId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();

                var affectedRows = connection.Execute(
                    "DELETE FROM Alert WHERE DeviceId = @DeviceId",
                    new { DeviceId = deviceId }
                );

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                throw new SqlTypeException("An error occurred while deleting alerts from device #" + deviceId, ex);
            }
        }
    }
}