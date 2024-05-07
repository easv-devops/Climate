using api;
using api.clientEventHandlers;
using api.clientEventHandlers.roomClientHandlers;
using api.helpers;
using api.ServerEventHandlers;
using api.serverEventModels;
using infrastructure.Models;
using tests.WebSocket;

namespace tests;
public class ClientWantsToEditRoomTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase("user@example.com", "12345678",1, "Basement", TestName = "Valid")]
    [TestCase("user@example.com", "12345678",1, "Way too long name for a room that can be maximum 50 characters", TestName = "NameTooLong")]
    [TestCase("user@example.com", "12345678",5, "Bedroom", TestName = "Not user's room")]
    public async Task EditRoomTest(string email, string password, int roomId, string roomName)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });
        
        ws.Send(new ClientWantsToGetAllRoomsDto(){});
        
        var room = new RoomWithId
        {
            Id = roomId,
            RoomName = roomName
        };
        
        await ws.DoAndAssert(new ClientWantsToEditRoomDto()
        {
            RoomToEdit = room
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
                    
                    case "Not user's room":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    default:
                        return false;
                }
            }) == 1;
        });
    }
    
    [Test]
    public async Task EditRoomTestUnauthorized()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        
        var room = new RoomWithId
        {
            Id = 1,
            RoomName = "Bedroom"
        };
        
        await ws.DoAndAssert(new ClientWantsToEditRoomDto()
        {
            RoomToEdit = room
        }, fromServer =>
        {
            return fromServer.Count(dto => dto.eventType == nameof(ServerSendsErrorMessageToClient)) == 1;
        });
    }
}