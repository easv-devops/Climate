using api;
using api.clientEventHandlers;
using api.clientEventHandlers.AlertClientHandlers;
using api.helpers;
using api.ServerEventHandlers;
using api.serverEventModels;
using infrastructure.Models;
using tests.WebSocket;

namespace tests;

public class AlertTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase(1, 10, 50, 5, 5, TestName = "Temperature too low")]
    [TestCase(1, 20, 99, 5, 5, TestName = "Humidity too high")]
    [TestCase(1, 20, 50, 20, 5, TestName = "P25 too high")]
    [TestCase(1, 20, 50, 5, 20, TestName = "P100 too high")]
    public async Task ScreenReadingsTests(int deviceId, double temperature, double humidity, int p25, int p100)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        ws.Send(new ClientWantsToSignInDto
        {
            email = "user@example.com",
            password = "12345678"
        });

        var deviceData = GenerateDeviceData(deviceId, temperature, humidity, p25, p100);
        
        await ws.DoAndAssert(new ClientWantsToCreateAlertDto()
        {
            DeviceData = deviceData
        }, fromServer =>
        {
            return fromServer.Count(dto =>
            {
                string testName = TestContext.CurrentContext.Test.Name;
                switch (testName)
                {
                    case "Temperature too low":
                        return dto.eventType == nameof(ServerSendsAlertList);
                    
                    case "Humidity too high":
                        return dto.eventType == nameof(ServerSendsAlertList);
                    
                    case "P25 too high":
                        return dto.eventType == nameof(ServerSendsAlertList);

                    case "P100 too high":
                        return dto.eventType == nameof(ServerSendsAlertList);

                    default:
                        return false;
                }
            }) == 2; // First ServerSendsAlertList is triggered on login. Second from ClientWantsToCreateAlertDto
        });
    }
    
    [Test]
    public async Task ScreenReadingsTestsNoAlerts()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        ws.Send(new ClientWantsToSignInDto
        {
            email = "user@example.com",
            password = "12345678"
        });

        var deviceData = GenerateDeviceData(1, 20, 50, 5, 5);
        
        await ws.DoAndAssert(new ClientWantsToCreateAlertDto()
        {
            DeviceData = deviceData
        }, fromServer =>
        {
            // Wait 2 secs to make sure server has a chance to respond
            // (condition is instantly met, as we expect no response of type ServerSendsAlertList)
            Task.Delay(2000); 
            return fromServer.All(dto => dto.eventType != nameof(ServerSendsAlertList)); 
        });
    }
    
    [Test]
    public async Task ClientWantsToGetAlertsTest()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        // Arrange: Log in and mock new device data
        ws.Send(new ClientWantsToSignInDto
        {
            email = "user@example.com",
            password = "12345678"
        });

        var deviceData = GenerateDeviceData(1, 20, 50, 5, 20); // P100 too high
        
        ws.Send(new ClientWantsToCreateAlertDto()
        {
            DeviceData = deviceData
        });
        
        // Act: Get unread alerts
        await ws.DoAndAssert(new ClientWantsToGetAlertsDto()
        {
            IsRead = false
        }, fromServer =>
        {
            return fromServer.Count(dto => dto.eventType == nameof(ServerSendsAlertList)) == 3; 
            // First ServerSendsAlertList is triggered on login.
            // Second from ClientWantsToCreateAlertDto.
            // Third from ClientWantsToGetAlertsDto
        });
    }
    
    [Test]
    public async Task ClientWantsToEditAlertTest()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        // Arrange: Sign in & generate alerts
        ws.Send(new ClientWantsToSignInDto
        {
            email = "user@example.com",
            password = "12345678"
        });
        
        var deviceData = GenerateDeviceData(1, 35, 50, 5, 5);

        ws.Send(new ClientWantsToCreateAlertDto()
        {
            DeviceData = deviceData
        });
        
        // Act: Edit alert (mark as Read)
        await ws.DoAndAssert(new ClientWantsToEditAlertDto()
        {
            AlertId = 1,
            DeviceId = 1,
            IsRead = true
        }, fromServer =>
        { // Assert: Expecting ServerSendsAlert event for the edited alert
            return fromServer.Count(dto => dto.eventType == nameof(ServerSendsAlert)) == 1;
        });
        
    }

    private DeviceData GenerateDeviceData(int deviceId, double temperature, double humidity, int p25, int p100)
    {
        var deviceData = new DeviceData()
        {
            DeviceId = deviceId,
            Data = new DeviceReadingsDto()
            {
                Temperatures = GenerateSensorList(temperature),
                Humidities = GenerateSensorList(humidity),
                Particles25 = GenerateSensorList(p25),
                Particles100 = GenerateSensorList(p100)
            }
        };

        return deviceData;
    }
    
    private List<SensorDto> GenerateSensorList(double value)
    {
        return new List<SensorDto> { new() { Value = value, TimeStamp = "2024-05-23 14:00:00" } };
    }
}