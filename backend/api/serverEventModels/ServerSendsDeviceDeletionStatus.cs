using lib;

namespace api.serverEventModels;

public class ServerSendsDeviceDeletionStatus : BaseDto
{
    public required bool IsDeleted { get; set; }
}