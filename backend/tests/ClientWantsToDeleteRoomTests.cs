using api;
using api.clientEventHandlers;
using api.clientEventHandlers.roomClientHandlers;
using api.helpers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;
public class ClientWantsToDeleteRoomTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase("user@example.com", "12345678",1, TestName = "Valid")]
    [TestCase("user@example.com", "12345678",5, TestName = "Not user's room")]
    public async Task DeleteRoomTest(string email, string password, int roomId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });
        
        await ws.DoAndAssert(new ClientWantsToDeleteRoomDto()
        {
            RoomToDelete = roomId
        }, fromServer =>
        {
            return fromServer.Count(dto =>
            {
                string testName = TestContext.CurrentContext.Test.Name;
                switch (testName)
                {
                    case "Valid":
                        Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                            serverEvent => serverEvent.eventType == nameof(ServerDeletesRoom)));
                        return dto.eventType == nameof(ServerDeletesRoom);
                    
                    case "Not user's room":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    default:
                        return false;
                }
            }) == 1;
        });
    }
    
    [Test]
    public async Task DeleteRoomTestUnauthorized()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        // Arrange (no login)
        // Act
        await ws.DoAndAssert(new ClientWantsToDeleteRoomDto()
        {
            RoomToDelete = 1
        }, fromServer =>
        { 
            // Assert
            return fromServer.Count(dto => dto.eventType == nameof(ServerSendsErrorMessageToClient)) == 1;
        });
    }
}