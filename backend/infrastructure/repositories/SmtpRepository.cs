using System.Net;
using System.Net.Mail;
using infrastructure.Models;

namespace infrastructure;

public class SmtpRepository
{
    public bool SendEmail(EmailDto emailContent)
    {
        using (var client = new SmtpClient())
        {
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("climatenoti@GMAIL.COM", KeyVaultService.GetMailPassword());

            using (var message = new MailMessage(
                       from: new MailAddress("climatenoti@GMAIL.COM", "Your Climate"),
                       to: new MailAddress(emailContent.recipientEmail)
                   ))
            {
                message.Subject =  emailContent.subject;
                message.Body =  emailContent.htmlBody;
                message.IsBodyHtml = true;

                client.Send(message);
            }
        }
        return true;
    }
}