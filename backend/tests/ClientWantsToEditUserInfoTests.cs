using api;
using api.clientEventHandlers;
using api.helpers;
using api.ServerEventHandlers;
using api.serverEventModels;
using infrastructure.Models;
using tests.WebSocket;

namespace tests;

public class ClientWantsToEditUserInfoTests
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }

    [TestCase(1, "Fornavn", "Efternavn", "emailWithNoAt", "DK", "10101010", TestName = "Invalid Email")]
    [TestCase(1, "Fornavn", "Efternavn", "ny@email.com", "DK", "abcdefgh", TestName = "Invalid Number")]
    public async Task ClientWantsToEditUserInfoInvalid(int userId, string firstName, string lastName, string email,
        string countryCode, string phoneNumber)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange (login)
        ws.Send(new ClientWantsToSignInDto()
        {
            email = "user@example.com",
            password = "12345678"
        });

        //Act
        await ws.DoAndAssert(new ClientWantsToEditUserInfoDto()
            {
                UserDto = new FullUserDto()
                {
                    Id = userId,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    CountryCode = countryCode,
                    Number = phoneNumber
                }
            },
            //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    string testName = TestContext.CurrentContext.Test.Name;
                    switch (testName)
                    {
                        case "Invalid Email":
                            Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                                serverEvent => serverEvent.eventType == nameof(ServerSendsErrorMessageToClient)));
                            return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                        case "Invalid Number":
                            Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                                serverEvent => serverEvent.eventType == nameof(ServerSendsErrorMessageToClient)));
                            return dto.eventType == nameof(ServerSendsErrorMessageToClient);
                        default:
                            return false;
                    }
                }) == 1;             }
        );
    }

    [Test]
    public async Task ClientWantsToEditUserInfoValid()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        //Arrange (login)
        ws.Send(new ClientWantsToSignInDto()
        {
            email = "user@example.com",
            password = "12345678"
        });

        //Act
        await ws.DoAndAssert(new ClientWantsToEditUserInfoDto()
            {
                UserDto = new FullUserDto()
                {
                    Id = 1,
                    Email = "ny@email.com",
                    FirstName = "Fornavn",
                    LastName = "Efternavn",
                    CountryCode = "US",
                    Number = "10101010"
                }
            },
            //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count(
                        serverEvent => serverEvent.eventType == nameof(ServerSendsUser)));
                    return dto.eventType == nameof(ServerSendsUser);
                }) == 2; // Expecting 2 ServerSendsUser as ClientWantsToSignInDto triggers the first
            }
        );
    }
}