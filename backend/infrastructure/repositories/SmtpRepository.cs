using System.Net;
using System.Net.Mail;
using api.helpers;
using infrastructure.Models;

namespace infrastructure;

public class SmtpRepository
{
    public bool SendEmail1(string body)
    {
        using (var client = new SmtpClient())
        {
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("climatenoti@GMAIL.COM", Environment.GetEnvironmentVariable(EnvVarKeys.MailPassword.ToString()));
            using (var message = new MailMessage(
                       from: new MailAddress("climatenoti@GMAIL.COM", "Your Climate"),
                       to: new MailAddress("kennidamkier@gmail.com", "kenni")
                   ))
            {

                message.Subject = "Reset Password";
                message.Body = body;

                client.Send(message);
            }
        }
        return true;
    }
    
    public bool SendEmail(EmailDto emailContent)
    {
        using (var client = new SmtpClient())
        {
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("climatenoti@GMAIL.COM", Environment.GetEnvironmentVariable(EnvVarKeys.MailPassword.ToString()));

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