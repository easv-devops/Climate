using api.helpers;
using infrastructure;
using infrastructure.Models;

namespace service.services.notificationServices;

public class NotificationService
{
    private SmtpRepository _smtpRepository;

    public NotificationService(SmtpRepository smtpRepository)
    {
        _smtpRepository = smtpRepository;
    }
    
    /**
     * checks notification preference and call relevant methods for preferences.
     * Include other message types in switch case if needed (Dont know if we want sms)
     */
    public bool SendWelcomeMessage(List<MessageType> notiPreferences, UserRegisterDto user)
    {
        if (notiPreferences == null || !notiPreferences.Any())
            throw new Exception("no message types selected");
        
        foreach (var messageType in notiPreferences)
        {
            switch (messageType)
            {
                case MessageType.EMAIL:
                    if (!SendWelcomeEmail(user))
                    {
                        return false;
                    }
                    break;
                default:
                    throw new NotImplementedException("notification preference not implemented");
            }
        }
        return true;
    }

    private bool SendWelcomeEmail(UserRegisterDto user)
    {
        var mailBuilder = new EmailBuilder();

        mailBuilder.BuildWelcomeMessage(user.FullName);
        throw new NotImplementedException();
    }
}