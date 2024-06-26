﻿using api;
using api.clientEventHandlers;
using api.helpers;
using api.serverEventModels;
using tests.WebSocket;

namespace tests;
public class ClientWantsToEditDeviceTests
{
        
    [SetUp]
    public void Setup()
    {
        FlywayDbTestRebuilder.ExecuteMigrations();
        Startup.Start(null, Environment.GetEnvironmentVariable(EnvVarKeys.dbtestconn.ToString()));
    }
    

    [TestCase(1, "user@example.com", "12345678","navnpådevice", "1", TestName = "ValidRoomId")]
    [TestCase(5, "user@example.com", "12345678","jfiewfwe", "1", TestName = "InvalidDeviceId")]
    public async Task EditDeviceTest(int id, string email, string password, string deviceName, int roomId)
    {
        var ws = await new WebSocketTestClient().ConnectAsync();

        ws.Send(new ClientWantsToSignInDto
        {
            email = email,
            password = password
        });
        
        ws.Send(new ClientWantsToCreateDeviceDto
        {
            DeviceName = deviceName,
            RoomId = roomId
        });
        
        await ws.DoAndAssert(new ClientWantsToEditDeviceDto
        {
            Id = id,
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
                
                    case "InvalidDeviceId":
                        return dto.eventType == nameof(ServerSendsErrorMessageToClient);

                    default:
                        return false;
                }
            }) == 1; // You can adjust this condition based on your requirements.
        });
    }
}