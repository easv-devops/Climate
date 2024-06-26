﻿using api;
using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using tests.WebSocket;
namespace tests;

public class ClientWantsToCreateDevice
{
    
    
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }
    

    [TestCase("user@example.com", "12345678","navnpådevice", "1", TestName = "ValidRoomId")]
    [TestCase("user@example.com", "12345678","navnpådevice", "-1", TestName = "InvalidRoomId")]
    public async Task CreateDeviceTest(string email, string password, string deviceName, int roomId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });
        
        await ws.DoAndAssert(new ClientWantsToCreateDeviceDto
        {
            DeviceName = deviceName,
            RoomId = roomId
        }, fromServer =>
        {
            return fromServer.Count(dto =>
            {
                string testName = TestContext.CurrentContext.Test.Name;
                switch (testName)
                {
                    case "ValidRoomId":
                        return dto.eventType == nameof(ServerSendsDevice); // Replace "ValidEventType" with the expected eventType for Valid test.
                
                    case "InvalidRoomId":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    default:
                        return false;
                }
            }) == 1; // You can adjust this condition based on your requirements.
        });
    }

}