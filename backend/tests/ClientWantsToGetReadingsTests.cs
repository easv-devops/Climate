using api;
using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;

public class ClientWantsToGetReadingsTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase("user@example.com", "12345678", 1, TestName = "Valid")]
    [TestCase("user@example.com", "12345678", 5, TestName = "Not logged in user's device")]
    public async Task ClientWantsToGetTemperatureReadings(string email, string password, int deviceId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange: login & request devices (so they're mapped in State)
        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });
        
        //Act
        await ws.DoAndAssert(new ClientWantsToGetTemperatureReadingsDto
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
                                serverEvent => serverEvent.eventType == nameof(ServerSendsTemperatureReadings)));
                            return dto.eventType == nameof(ServerSendsTemperatureReadings);
                        
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

    [TestCase("user@example.com", "12345678", 1, TestName = "Valid")]
    [TestCase("user@example.com", "12345678", 5, TestName = "Not logged in user's device")]
    public async Task ClientWantsToGetHumidityReadings(string email, string password, int deviceId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange: login & request devices (so they're mapped in State)
        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });
        
        ws.Send(new ClientWantsToGetDevicesByUserIdDto{ });

        //Act
        await ws.DoAndAssert(new ClientWantsToGetHumidityReadingsDto
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
                                serverEvent => serverEvent.eventType == nameof(ServerSendsHumidityReadings)));
                            return dto.eventType == nameof(ServerSendsHumidityReadings);
                        
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

    [TestCase("user@example.com", "12345678", 1, TestName = "Valid")]
    [TestCase("user@example.com", "12345678", 5, TestName = "Not logged in user's device")]
    public async Task ClientWantsToGetPm25Readings(string email, string password, int deviceId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange: login & request devices (so they're mapped in State)
        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });
        
        ws.Send(new ClientWantsToGetDevicesByUserIdDto{ });

        //Act
        await ws.DoAndAssert(new ClientWantsToGetPm25ReadingsDto
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
                                serverEvent => serverEvent.eventType == nameof(ServerSendsPm25Readings)));
                            return dto.eventType == nameof(ServerSendsPm25Readings);
                        
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

    [TestCase("user@example.com", "12345678", 1, TestName = "Valid")]
    [TestCase("user@example.com", "12345678", 5, TestName = "Not logged in user's device")]
    public async Task ClientWantsToGetPm100Readings(string email, string password, int deviceId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange: login & request devices (so they're mapped in State)
        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });
        
        ws.Send(new ClientWantsToGetDevicesByUserIdDto{ });

        //Act
        await ws.DoAndAssert(new ClientWantsToGetPm100ReadingsDto
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
                                serverEvent => serverEvent.eventType == nameof(ServerSendsPm100Readings)));
                            return dto.eventType == nameof(ServerSendsPm100Readings);
                        
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

    [TestCase(1, TestName = "Temperature")]
    [TestCase(1, TestName = "Humidity")]
    [TestCase(1, TestName = "Pm25")]
    [TestCase(1, TestName = "Pm100")]
    public async Task UnauthorizedClientWantsToGetDevices(int id)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //No login, so no JWT.

        string testName = TestContext.CurrentContext.Test.Name;
        switch (testName)
        {
            case "Temperature":
                //Act
                await ws.DoAndAssert(new ClientWantsToGetTemperatureReadingsDto
                    {
                        DeviceId = id
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
                    }
                );
                break;
            case "Humidity":
                //Act
                await ws.DoAndAssert(new ClientWantsToGetHumidityReadingsDto
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
                    }
                );
                break;
            case "Pm25":
                //Act
                await ws.DoAndAssert(new ClientWantsToGetPm25ReadingsDto
                    {
                        DeviceId = id
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
                    }
                );
                break;
            case "Pm100":
                //Act
                await ws.DoAndAssert(new ClientWantsToGetPm100ReadingsDto
                    {
                        DeviceId = id
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
                    }
                );
                break;
        }
    }
}