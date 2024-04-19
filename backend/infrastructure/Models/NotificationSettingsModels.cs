namespace infrastructure.Models;

public enum MessageType
{
    EMAIL,
    SMS
}

public class NotificationSettingsModels
{
    
}

public class EmailDto
{
    public string subject { get; set; }
    public string htmlBody { get; set; }
    public string recipientEmail { get; set; }
}