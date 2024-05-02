using api.clientEventHandlers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;

public class ClientWantsAllTheirRoomsTest
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null);
    }

    [TestCase("user@example.com", "12345678", 1, TestName = "Valid")]
    public async Task ClientWantsAllRooms(string email, string password, int userId)
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
                await ws.DoAndAssert(new ClientWantsAllRoomsDto()
                    {
                        UserId = userId
                    },

                    //Assert
                    fromServer =>
                    {
                        return fromServer.Count(dto =>
                        {
                            Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                                serverEvent => serverEvent.eventType == nameof(ServerReturnsAllRooms)));

                            return dto.eventType == nameof(ServerReturnsAllRooms);
                        }) == 1;
                    }
                );
                break;
        }
    }
}