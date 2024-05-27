using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsAlertList : BaseDto
{
    public IEnumerable<AlertDto>? Alerts { get; set; }
}