using api;
using api.clientEventHandlers;
using api.clientEventHandlers.roomClientHandlers;
using api.helpers;
using api.ServerEventHandlers;
using api.serverEventModels;
using infrastructure.Models;
using tests.WebSocket;
namespace tests;

public class ClientWantsToCreateRoomTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase("user@example.com", "12345678","Basement", TestName = "Valid")]
    [TestCase("user@example.com", "12345678","The room that is next to that other room in my very big house", TestName = "NameTooLong")]
    public async Task CreateRoomTest(string email, string password, string roomName)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });

        var room = new RoomWithNoId
        {
            RoomName = roomName
        };
        
        await ws.DoAndAssert(new ClientWantsToCreateRoomDto()
        {
            RoomToCreate = room
        }, fromServer =>
        {
            return fromServer.Count(dto =>
            {
                string testName = TestContext.CurrentContext.Test.Name;
                switch (testName)
                {
                    case "Valid":
                        return dto.eventType == nameof(ServerSendsRoom);
                
                    case "NameTooLong":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    default:
                        return false;
                }
            }) == 1;
        });
    }

    [Test]
    public async Task CreateRoomTestUnauthorized()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        // Arrange (no login + instantiate the room to create)
        var room = new RoomWithNoId
        {
            RoomName = "Kitchen"
        };
        
        // Act
        await ws.DoAndAssert(new ClientWantsToCreateRoomDto()
        {
            RoomToCreate = room
        }, fromServer =>
        { 
            // Assert
            return fromServer.Count(dto => dto.eventType == nameof(ServerSendsErrorMessageToClient)) == 1;
        });
    }
}