using api;
using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;

public class ClientWantsToGetLatestReadingsTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }
    
    [TestCase(1, TestName = "Valid")]
    [TestCase(5, TestName = "Not logged in user's device")]
    public async Task ClientWantsToGetLatestDeviceReadingsTests(int deviceId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange: login & request devices (so they're mapped in State)
        ws.Send(new ClientWantsToSignInDto
        {
            email = "user@example.com",
            password = "12345678"
        });
        
        //Act
        await ws.DoAndAssert(new ClientWantsToGetLatestDeviceReadingsDto
            {
                DeviceId = deviceId
            },
            //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    string testName = TestContext.CurrentContext.Test.Name;
                    switch (testName)
                    {
                        case "Valid":
                            Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                                serverEvent => serverEvent.eventType == nameof(ServerSendsLatestDeviceReadings)));
                            return dto.eventType == nameof(ServerSendsLatestDeviceReadings);
                        
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
    
    [TestCase(1, TestName = "Valid")]
    [TestCase(5, TestName = "Not logged in user's room")]
    public async Task ClientWantsToGetLatestRoomReadingsTests(int roomId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange: login & request devices (so they're mapped in State)
        ws.Send(new ClientWantsToSignInDto
        {
            email = "user@example.com",
            password = "12345678"
        });
        
        //Act
        await ws.DoAndAssert(new ClientWantsToGetLatestRoomReadingsDto()
            {
                RoomId = roomId
            },
            //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    string testName = TestContext.CurrentContext.Test.Name;
                    switch (testName)
                    {
                        case "Valid":
                            Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                                serverEvent => serverEvent.eventType == nameof(ServerSendsLatestRoomReadings)));
                            return dto.eventType == nameof(ServerSendsLatestRoomReadings);
                        
                        case "Not logged in user's room":
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
}

