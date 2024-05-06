using api;
using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;

public class ClientWantsToAuthenticateTest
{
    [SetUp]
    public void Setup()
    {
       FlywayDbTestRebuilder.ExecuteMigrations();
       Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
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
    
    [TestCase("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9.eyJzdWIiOjEsImV4cCI6MTgwMTM5NDI2OX0.u-80Cb2ysefJsMwP_YnkltvN5pQkI2IIJmuOfPy9ITBvC-QYaASiWVJbCe31EUSplqnknPHqhS6Gm1-d7qk6kA", TestName = "Valid")]
    [TestCase("eyYOthisISaFAKEjwt", TestName = "Invalid")]
    public async Task LoginWithJwtTest(string jwt)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        await ws.DoAndAssert(new ClientWantsToAuthenticateWithJwtDto
        {
            jwt = jwt
        }, fromServer =>
        {
            return fromServer.Count(dto =>
            {
                string testName = TestContext.CurrentContext.Test.Name;
                switch (testName)
                {
                    case "Valid":
                        return dto.eventType == nameof(ServerAuthenticatesUser);
                    case "Invalid":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                    default:
                        return false;
                }
            }) == 1;
        });
    }
}