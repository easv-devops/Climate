using api;
using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using infrastructure.Models;
using tests.WebSocket;

namespace tests;

public class ClientWantsToEditRangeSettingsForDeviceTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase(1, 23, 16, 60, 40, 9, 9, TestName = "Valid")]
    [TestCase(1, 50, 16, 60, 40, 9, 9, TestName = "Invalid max. temp")]
    [TestCase(1, 23, 16, 60, -20, 9, 9, TestName = "Invalid min. hum")]
    [TestCase(1, 23, 16, 60, 40, 9, 101, TestName = "Invalid max. p100")]
    public async Task ClientWantsToEditDeviceSettingsFromId(int deviceId, int tempMax, int tempMin, int humMax, int humMin,
        int p25Max, int p100Max)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        ws.Send(new ClientWantsToSignInDto
        {
            email = "user@example.com",
            password = "12345678"
        });

        var ranges = new RangeDto()
        {
            Id = deviceId,
            TemperatureMax = tempMax,
            TemperatureMin = tempMin,
            HumidityMax = humMax,
            HumidityMin = humMin,
            Particle25Max = p25Max,
            Particle100Max = p100Max
        };

        await ws.DoAndAssert(new ClientWantsToEditDeviceRangeDto()
        {
            Settings = ranges
        }, fromServer =>
        {
            return fromServer.Count(dto =>
            {
                string testName = TestContext.CurrentContext.Test.Name;
                switch (testName)
                {
                    case "Valid":
                        Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                            serverEvent => serverEvent.eventType == nameof(ServerSendsDeviceRangeSettings)));
                        return dto.eventType == nameof(ServerSendsDeviceRangeSettings);

                    case "Invalid max. temp":
                        Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                            serverEvent => serverEvent.eventType == nameof(ServerSendsErrorMessageToClient)));
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    case "Invalid min. hum":
                        Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                            serverEvent => serverEvent.eventType == nameof(ServerSendsErrorMessageToClient)));
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    case "Invalid max. p100":
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

        var ranges = new RangeDto()
        {
            Id = 1,
            TemperatureMax = 24,
            TemperatureMin = 16,
            HumidityMax = 60,
            HumidityMin = 40,
            Particle25Max = 9,
            Particle100Max = 9
        };

        await ws.DoAndAssert(new ClientWantsToEditDeviceRangeDto()
        {
            Settings = ranges
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