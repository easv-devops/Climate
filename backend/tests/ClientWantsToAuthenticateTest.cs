using api.clientEventHandlers;
using api.serverEventModels;
using infrastructure.Models.serverEvents;
using tests.WebSocket;

namespace tests;

public class ClientWantsToAuthenticateTest
{
    [SetUp]
    public void Setup()
    {
       FlywayDbTestRebuilder.ExecuteMigrations();
       Startup.Start(null, "dbtestconn");
    }

    [TestCase("user@example.com", "12345678", TestName = "Valid")]
    [TestCase("user@example.com", "87651", TestName = "Invalid password")]
    [TestCase("userAbTexample.com", "12345678", TestName = "Invalid email")]
    public async Task LoginTest(string email, string password)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        await ws.DoAndAssert(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        }, fromServer =>
        {
            return fromServer.Count(dto =>
            {
                Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count);
                string testName = TestContext.CurrentContext.Test.Name;
                switch (testName)
                {
                    case "Valid":
                        return dto.eventType == nameof(ServerAuthenticatesUser);
                    case "Invalid password":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                    case "Invalid email":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                    default:
                        return false;
                }
            }) == 1;
        });
    }
}