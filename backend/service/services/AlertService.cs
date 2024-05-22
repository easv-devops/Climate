using infrastructure.Models;
using infrastructure.repositories;

namespace service.services;

public class AlertService
{
    private readonly AlertRepository _alertRepository;
    
    public AlertService(AlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public AlertDto CreateAlert(CreateAlertDto dto)
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