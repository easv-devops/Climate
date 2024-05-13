using api;
using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;

public class ClientWantsToGetDeviceTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase("user@example.com", "12345678", TestName = "Valid")]
    public async Task ClientWantsToGetDevicesByUserId(string email, string password)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange (login)
        ws.Send(new ClientWantsToSignInDto()
        {
            email = email,
            password = password
        });

        //Act
        await ws.DoAndAssert(new ClientWantsToGetDevicesByUserIdDto()
            {
            },
            //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                        serverEvent => serverEvent.eventType == nameof(ServerSendsDevicesByUserId)));
                    return dto.eventType == nameof(ServerSendsDevicesByUserId);
                }) == 2; // Expecting 2 ServerSendsDevicesByUserId as ClientWantsToSignInDto triggers the first
            }
        );
    }

    [Test]
    public async Task UnauthorizedClientWantsToGetDevices()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Act
        await ws.DoAndAssert(new ClientWantsToGetDevicesByUserIdDto()
            {
            },
            //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                }) == 1;
            }
        );
    }
}