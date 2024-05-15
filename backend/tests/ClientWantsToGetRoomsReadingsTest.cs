using api;
using api.clientEventHandlers;
using api.clientEventHandlers.roomClientHandlers;
using api.helpers;
using api.serverEventModels;
using api.serverEventModels.roomDtos;
using tests.WebSocket;

namespace tests;

public class ClientWantsToGetRoomReadingsTest
{

    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase("user@example.com", "12345678", 1, TestName = "Valid")]
    public async Task ClientWantsToGetRoomTempTest(string email, string password, int roomId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange: login & request devices (so they're mapped in State)
        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });

        //Act
        await ws.DoAndAssert(new ClientWantsToGetTemperatureReadingsForRoomDto
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
                                serverEvent => serverEvent.eventType == nameof(ServerSendsTemperatureReadingsForRoom)));
                            return dto.eventType == nameof(ServerSendsTemperatureReadingsForRoom);

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
    public async Task ClientWantsToGetHumiRoomTest(string email, string password, int roomId)
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
        await ws.DoAndAssert(new ClientWantsToGetHumidityReadingsForRoomDto
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
                                serverEvent => serverEvent.eventType == nameof(ServerSendsHumidityReadingsForRoom)));
                            return dto.eventType == nameof(ServerSendsHumidityReadingsForRoom);
                        
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
    public async Task ClientWantsToGetPm25ReadingsRoomTest(string email, string password, int roomId)
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
        await ws.DoAndAssert(new ClientWantsToGetPm25ReadingsForRoomDto()
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
                                serverEvent => serverEvent.eventType == nameof(ServerSendsPm25ReadingsForRoom)));
                            return dto.eventType == nameof(ServerSendsPm25ReadingsForRoom);
                        
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
    public async Task ClientWantsToGetPm100ReadingsRoomTest(string email, string password, int roomId)
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
        await ws.DoAndAssert(new ClientWantsToGetPm100ReadingsForRoomDto()
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
                                serverEvent => serverEvent.eventType == nameof(ServerSendsPm100ReadingsForRoom)));
                            return dto.eventType == nameof(ServerSendsPm100ReadingsForRoom);
                        
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
}