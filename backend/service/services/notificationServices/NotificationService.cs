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
    public bool SendWelcomeMessage(List<MessageType> notiPreferences, ShortUserDto user)
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
                case MessageType.SMS:
                    break;
                default:
                    throw new NotImplementedException("notification preference not implemented");
            }
        }
        return true;
    }

    public bool SendResetPasswordMessage(MessageType messageType, string newPassword, string email)
    { 
        switch (messageType)
        {
            case MessageType.EMAIL:
                if (!SendResetPasswordEmail(newPassword, email))
                {
                    return false;
                }
                break;
            case MessageType.SMS:
                break;
            default:
                throw new NotImplementedException("notification preference not implemented");
        }

        return true;
    }

    private bool SendResetPasswordEmail(string newPassword, string email)
    {
        var mailBuilder = new EmailBuilder();
        string mailBody = mailBuilder.BuildResetPasswordMessage(newPassword);
        var mail = new EmailDto
        {
            subject = "Your Password Has Been Reset",
            htmlBody = mailBody,
            recipientEmail = email
        };
        return _smtpRepository.SendEmail(mail);
        
    }

    private bool SendWelcomeEmail(ShortUserDto user)
    {
        var mailBuilder = new EmailBuilder();
        string mailBody = mailBuilder.BuildWelcomeMessage(user.Name);

        var mail = new EmailDto
        {
            subject = "Welcome to Climate " + user.Name,
            htmlBody = mailBody,
            recipientEmail = user.Email
        };
        
        return _smtpRepository.SendEmail(mail);
    }
}