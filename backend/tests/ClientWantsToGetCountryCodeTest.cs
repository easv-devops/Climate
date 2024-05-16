using api;
using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;

public class ClientWantsToGetCountryCodeTest
{
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }
    
    
    [Test]
    public async Task ClientWantsToGetCountryCode()
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
        
        //Act
        await ws.DoAndAssert(new ClientWantsToGetCountryCodeDto{}, 
            
        //Assert
            fromServer =>
            {
                return fromServer.Count(dto =>
                {
                    return dto.eventType == nameof(ServerSendsCountryCodes);
                }) == 1;
            }
        );
    }
    
}