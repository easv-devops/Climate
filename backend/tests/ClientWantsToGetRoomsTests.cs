using api;
using api.clientEventHandlers;
using api.clientEventHandlers.roomClientHandlers;
using api.helpers;
using api.serverEventModels;
using api.serverEventModels.roomDtos;
using tests.WebSocket;

namespace tests;

public class ClientWantsToGetRoomsTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [Test]
    public async Task ClientWantsToGetRooms()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange (login)
        ws.Send(new ClientWantsToSignInDto()
        {
            email = "user@example.com",
            password = "12345678"
        });
        
        //Act
        await ws.DoAndAssert(new ClientWantsToGetAllRoomsDto{},
            //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                        serverEvent => serverEvent.eventType == nameof(ServerReturnsAllRooms)));
                    return dto.eventType == nameof(ServerReturnsAllRooms);
                }) == 2; // Expecting 2 ServerReturnsAllRooms as ClientWantsToSignInDto triggers the first
            }
        );
    }

    [Test]
    public async Task ClientWantsToGetRoomsUnauthorized()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange (no login)

        //Act
        await ws.DoAndAssert(new ClientWantsToGetAllRoomsDto{},
            //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                }) == 1;
            }
        );
    }
    
    
    
}