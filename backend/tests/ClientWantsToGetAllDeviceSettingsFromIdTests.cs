using api;
using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;

public class ClientWantsToGetAllDeviceSettingsFromIdTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase("user@example.com", "12345678", 1, TestName = "Valid Range Settings")]
    [TestCase("user@example.com", "12345678", 1, TestName = "Valid Device Settings")]
    [TestCase("user@example.com", "12345678", 5, TestName = "Not logged in user's device")]
    public async Task ClientWantsToGetAllDeviceSettingsFromId(string email, string password, int deviceId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange: login & request devices (so they're mapped in State)
        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });

        //Act
        await ws.DoAndAssert(new ClientWantsGetDeviceSettingsDto
            {
                Id = deviceId
            },
            //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    string testName = TestContext.CurrentContext.Test.Name;
                    switch (testName)
                    {
                        case "Valid Device Settings":
                            Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                                serverEvent => serverEvent.eventType == nameof(ServerSendsDeviceSettings)));
                            return dto.eventType == nameof(ServerSendsDeviceSettings);

                        case "Valid Range Settings":
                            Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                                serverEvent => serverEvent.eventType == nameof(ServerSendsDeviceRangeSettings)));
                            return dto.eventType == nameof(ServerSendsDeviceRangeSettings);

                        case "Not logged in user's device":
                            Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                                serverEvent => serverEvent.eventType == nameof(ServerSendsErrorMessageToClient)));
                            return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                        default:
                            return false;
                    }
                }) == 1;
            }
        );
    }

    [Test]
    public async Task UnauthorizedClientWantsToGetAllDeviceSettingsFromId()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Act
        await ws.DoAndAssert(new ClientWantsGetDeviceSettingsDto()
        {
        },
        //Assert
        fromServer =>
        {
            return fromServer.Count(dto =>
            {
                Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                    serverEvent => serverEvent.eventType == nameof(ServerSendsErrorMessageToClient)));
                return dto.eventType == nameof(ServerSendsErrorMessageToClient);
            }) == 1;
        });
    }
}