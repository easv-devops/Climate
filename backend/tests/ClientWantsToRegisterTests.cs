using api.clientEventHandlers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;

public class ClientWantsToRegisterTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null);
    }
    
    [TestCase("user102111@example.com", "12345678", "234567890", "John", "Doe", "+45", TestName = "Valid")]
    [TestCase("user50@example.com", "5",  "234567890", "John", "Doe", "+44", TestName = "Invalid password")]
    [TestCase("fewia.com", "156812333", "234567890", "John", "Doe", "+80", TestName = "invalid email")]
    public async Task RegisterTest(string email, string password,  string phone, string firstName, string lastName, string countryCode)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        await ws.DoAndAssert(new ClientWantsToRegisterDto
        {
            Email = email,
            Password = password,
            Phone = phone,
            FirstName = firstName,
            LastName = lastName,
            CountryCode = countryCode
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
                    case "invalid email":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                    default:
                        return false;
                }
            }) == 1;
        });
    }
}