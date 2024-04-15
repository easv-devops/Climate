using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class DbTestDto : BaseDto
{
    public string email { get; set; }
    public string password { get; set; }
}


public class TestDbConnection : BaseEventHandler<DbTestDto>
{
    
    private readonly AuthService _authService;

    public TestDbConnection(
        AuthService authService)
    {
        _authService = authService;

    }

    
    public override Task Handle(DbTestDto dto, IWebSocketConnection socket)
    {

        var test = _authService.TestConnection();

        if (test)
        {
            socket.Send("your db worked");
        }
        socket.Send("your db did not word");
        return Task.CompletedTask;
    }
}