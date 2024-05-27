using api;
using api.clientEventHandlers;
using api.helpers;
using tests.WebSocket;

namespace tests;

public class ClientWantsToGetDeviceIdsForRoomTest
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }
    
    [Test]
    public async Task ClientWantsToGetDeviceIdsForRoom()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        // Arrange: Log in
        ws.Send(new ClientWantsToSignInDto
        {
            email = "user@example.com",
            password = "12345678"
        });
        
        // Act: Get device ids for room 1
        await ws.DoAndAssert(new ClientWantsToGetDeviceIdsForRoomDto()
        {
            RoomId = 1
        }, fromServer =>
        {
            return fromServer.Count(dto => dto.eventType == nameof(ServerSendsDeviceIdListForRoom)) == 1; 
        });
    }
}