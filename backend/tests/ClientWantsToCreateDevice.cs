using api.clientEventHandlers;
using infrastructure.Models.serverEvents;
using tests.WebSocket;
namespace tests;

public class ClientWantsToCreateDevice
{
    
    
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null);
    }
    

    [TestCase("navnpådevice", "1", TestName = "ValidRoomId")]
    [TestCase("navnpådevice", "-0", TestName = "InvalidRoomId")]

    
    public async Task CreateDeviceTest(string deviceName, int roomId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();
    
        await ws.DoAndAssert(new ClientWantsToCreateDeviceDto
        {
            DeviceName = deviceName,
            RoomId = roomId
        }, fromServer =>
        {
            return fromServer.Count(dto =>
            {
                Console.WriteLine("Event type: " + dto.eventType + ". Count: " + fromServer.Count);
                string testName = TestContext.CurrentContext.Test.Name;
                switch (testName)
                {
                    case "Valid":
                        return dto.eventType == "ValidEventType"; // Replace "ValidEventType" with the expected eventType for Valid test.
                
                    case "Invalid":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    default:
                        return false;
                }
            }) == 1; // You can adjust this condition based on your requirements.
        });
    }

}