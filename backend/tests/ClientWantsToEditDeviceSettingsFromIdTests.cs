using api;
using api.clientEventHandlers;
using api.clientEventHandlers.roomClientHandlers;
using api.helpers;
using api.serverEventModels;
using infrastructure.Models;
using tests.WebSocket;

namespace tests;

public class ClientWantsToEditDeviceSettingsFromIdTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase(1, 5, 5, 5, TestName = "Valid")]
    [TestCase(1, -1, 5, 5, TestName = "Invalid BMP280 interval")]
    [TestCase(1, 5, -1, 5, TestName = "Invalid PMS interval")]
    [TestCase(1, 5, 5, -1, TestName = "Invalid Update interval")]
    public async Task ClientWantsToEditDeviceSettingsFromId(int deviceId, int bmp280ReadingInterval,
        int pmsReadingInterval, int updateInterval)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        ws.Send(new ClientWantsToSignInDto
        {
            email = "user@example.com",
            password = "12345678"
        });

        var settings = new SettingsDto()
        {
            Id = deviceId,
            BMP280ReadingInterval = bmp280ReadingInterval,
            PMSReadingInterval = pmsReadingInterval,
            UpdateInterval = updateInterval
        };

        await ws.DoAndAssert(new ClientWantsToEditDeviceSettingsDto()
        {
            Settings = settings
        }, fromServer =>
        {
            return fromServer.Count(dto =>
            {
                string testName = TestContext.CurrentContext.Test.Name;
                switch (testName)
                {
                    case "Valid":
                        Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                            serverEvent => serverEvent.eventType == nameof(ServerSendsDeviceSettings)));
                        return dto.eventType == nameof(ServerSendsDeviceSettings);

                    case "Invalid BMP280 interval":
                        Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                            serverEvent => serverEvent.eventType == nameof(ServerSendsErrorMessageToClient)));
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    case "Invalid PMS interval":
                        Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                            serverEvent => serverEvent.eventType == nameof(ServerSendsErrorMessageToClient)));
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    case "Invalid Update interval":
                        Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                            serverEvent => serverEvent.eventType == nameof(ServerSendsErrorMessageToClient)));
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    default:
                        return false;
                }
            }) == 1;
        });
    }

    [Test]
    public async Task UnauthorizedClientWantsToEditDeviceSettingsFromId()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        var settings = new SettingsDto()
        {
            Id = 1,
            BMP280ReadingInterval = 5,
            PMSReadingInterval = 5,
            UpdateInterval = 5
        };

        await ws.DoAndAssert(new ClientWantsToEditDeviceSettingsDto()
        {
            Settings = settings
        }, fromServer =>
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