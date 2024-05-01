using api;
using api.clientEventHandlers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;

public class ClientWantsToResetPassword
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, "");    }
    
    [TestCase("user@example.com", TestName = "Valid")]
    [TestCase("userDoesNotExist@example.com", TestName = "Invalid user")]
    [TestCase("fewia.com", TestName = "invalid email")]
    public async Task ResetPasswordTest(string email)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        await ws.DoAndAssert(new ClientWantsToResetPasswordDto
        {
            Email = email
        }, fromServer =>
        {
            return fromServer.Count(dto =>
            {
                string testName = TestContext.CurrentContext.Test.Name;
                switch (testName)
                {
                    case "Valid":
                        return dto.eventType == nameof(ServerResetsPassword);
                    case "Invalid user":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                    case "invalid email":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                    default:
                        return false;
                }
            }) == 1;
        });
    }
    
}