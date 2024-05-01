using api.clientEventHandlers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;

public class ClientWantsToGetDeviceTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, "");
    }

    [TestCase("user@example.com", "12345678", 1, TestName = "Valid")]
    [TestCase("user@example.com", "12345678", 5, TestName = "Not logged in user's device")]
    public async Task ClientWantsToGetDeviceById(string email, string password, int deviceId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange (login)
        ws.Send(new ClientWantsToSignInDto()
        {
            email = email,
            password = password
        });

        string testName = TestContext.CurrentContext.Test.Name;
        switch (testName)
        {
            case "Valid":
                //Act
                await ws.DoAndAssert(new ClientWantsToGetDeviceByIdDto()
                    {
                        DeviceId = deviceId
                    },
                    //Assert
                    fromServer =>
                    {
                        return fromServer.Count(dto =>
                        {
                            return dto.eventType == nameof(ServerSendsDeviceById);
                        }) == 1;
                    }
                );
                break;
            case "Not logged in user's device":
                //Act
                await ws.DoAndAssert(new ClientWantsToGetDeviceByIdDto()
                    {
                        DeviceId = deviceId
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
                break;
        }
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
                    return dto.eventType == nameof(ServerSendsDevicesByUserId);
                }) == 1;
            }
        );
    }

    [TestCase("user@example.com", "12345678", 1, TestName = "Valid")]
    [TestCase("user@example.com", "12345678", 5, TestName = "Not logged in user's room")]
    public async Task ClientWantsToGetDevicesByRoomId(string email, string password, int roomId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange (login)
        ws.Send(new ClientWantsToSignInDto()
        {
            email = email,
            password = password
        });

        //Act
        await ws.DoAndAssert(new ClientWantsToGetDevicesByRoomIdDto()
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
                            return dto.eventType == nameof(ServerSendsDevicesByRoomId);
                        case "Not logged in user's room": 
                            return dto.eventType == nameof(ServerSendsErrorMessageToClient); 
                        default:
                            return false;
                    }
                }) == 1;
            }
        );
    }

    [TestCase(1, TestName = "ById")]
    [TestCase(1, TestName = "ByUserId")]
    [TestCase(1, TestName = "ByRoomId")]
    public async Task UnauthorizedClientWantsToGetDevices(int id)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //No login, so no JWT.

        string testName = TestContext.CurrentContext.Test.Name;
        switch (testName)
        {
            case "ById":
                //Act
                await ws.DoAndAssert(new ClientWantsToGetDeviceByIdDto()
                    {
                        DeviceId = id
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
                break;
            case "ByUserId":
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
                break;
            case "ByRoomId":
                //Act
                await ws.DoAndAssert(new ClientWantsToGetDevicesByRoomIdDto()
                    {
                        RoomId = id
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
                break;
        }
    }
}