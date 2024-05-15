using api;
using api.clientEventHandlers;
using api.helpers;
using api.ServerEventHandlers;
using tests.WebSocket;

namespace tests;

public class ClientWantsToGetUserInfoTest
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }
    
    [Test]
    public async Task ClientWantsToGetUserInfo()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange (login)
        ws.Send(new ClientWantsToSignInDto()
        {
            email = "user@example.com",
            password = "12345678"
        });

        //Act
        await ws.DoAndAssert(new ClientWantsToGetUserInfoDto
            {
            },
            //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                        serverEvent => serverEvent.eventType == nameof(ServerSendsUser)));
                    return dto.eventType == nameof(ServerSendsUser);
                }) == 2; // Expecting 2 ServerSendsUser as ClientWantsToSignInDto triggers the first
            }
        );
    }
}