using System.Text.Json;
using api.clientEventHandlers.roomClientHandlers;
using api.helpers;
using api.serverEventModels;
using api.serverEventModels.roomDtos;
using api.WebSocket;
using Fleck;
using infrastructure;
using infrastructure.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using service.services;

namespace api.mqttEventListeners;

public class MqttClientSubscriber
{
    private DeviceReadingsService _readingsService;
    private readonly RoomReadingsService _roomReadingsService;
    private readonly DeviceService _deviceService;
    
    public MqttClientSubscriber(DeviceReadingsService readingsService, RoomReadingsService roomReadingsService, DeviceService deviceService)
    {
        _readingsService = readingsService;
        _roomReadingsService = roomReadingsService;
        _deviceService = deviceService;
    }
    
    public async Task CommunicateWithBroker()
    {

        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();
        
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("mqtt.flespi.io", 1883)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .WithCredentials(await KeyVaultService.GetMqttToken(), "")
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic("Climate/#"))
            .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var message = e.ApplicationMessage.ConvertPayloadToString();
           
                var messageObject = JsonSerializer.Deserialize<DeviceData>(message);

                _readingsService.CreateReadings(messageObject);
                
                SendDeviceReadingsToClient(messageObject);
                SendLatestDeviceReadingsToClient(messageObject);
                SendRoomReadingsToClient(messageObject);
                SendLatestRoomReadingsToClient(messageObject.DeviceId);
                
                //todo check for current listeners in state service and call relevant server to client handlers
                var pongMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("response_topic")//todo do we want some confirm? 
                    .WithPayload("yes we received the message, thank you very much, " +
                                 "the websocket client(s) also has the data")
                    .WithQualityOfServiceLevel(e.ApplicationMessage.QualityOfServiceLevel)
                    .WithRetainFlag(e.ApplicationMessage.Retain)
                    .Build();
                await mqttClient.PublishAsync(pongMessage, CancellationToken.None);
            }
            catch (Exception exc)
            {
                throw new ConnectionNotAvailableException("could not get data from Mqtt Broker", exc);
            }
        };
    }

    private void SendDeviceReadingsToClient(DeviceData messageObject)
    {
        // Sends the latest device readings to any active clients subscribed to the device
        var subscribedUserList = StateService.GetUsersForDevice(messageObject.DeviceId);

        if (subscribedUserList.Count == 0)
            return; // No subscribed listeners, stop here.
        
        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetClient(user);
            if (!ReferenceEquals(connection, null))
            {
                connection.Connection.SendDto(new ServerSendsTemperatureReadings()
                {
                    DeviceId = messageObject.DeviceId,
                    TemperatureReadings = messageObject.Data.Temperatures
                });
                
                connection.Connection.SendDto(new ServerSendsHumidityReadings()
                {
                    DeviceId = messageObject.DeviceId,
                    HumidityReadings = messageObject.Data.Humidities
                });
                
                connection.Connection.SendDto(new ServerSendsPm25Readings()
                {
                    DeviceId = messageObject.DeviceId,
                    Pm25Readings = messageObject.Data.Particles25
                });
                
                connection.Connection.SendDto(new ServerSendsPm100Readings()
                {
                    DeviceId = messageObject.DeviceId,
                    Pm100Readings = messageObject.Data.Particles100
                });
            }
        }
    }

    private void SendLatestDeviceReadingsToClient(DeviceData messageObject)
    {
        // Sends the latest device readings to any active clients subscribed to the device
        var subscribedUserList = StateService.GetUsersForDevice(messageObject.DeviceId);

        if (subscribedUserList.Count == 0)
            return; // No subscribed listeners, stop here.
        
        var latestReadings = new LatestData()
        {
            Id = messageObject.DeviceId,
            Data = new LatestReadingsDto()
            {
                Temperature = messageObject.Data.Temperatures.LastOrDefault(),
                Humidity = messageObject.Data.Humidities.LastOrDefault(),
                Particle25 = messageObject.Data.Particles25.LastOrDefault(),
                Particle100 = messageObject.Data.Particles100.LastOrDefault()
            }
        };
        
        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetClient(user);
            if (!ReferenceEquals(connection, null))
            {
                connection.Connection.SendDto(new ServerSendsLatestDeviceReadings()
                {
                    Data = latestReadings
                });
            }
        }
    }
    
    private void SendRoomReadingsToClient(DeviceData messageObject)
    {
        var roomId = _deviceService.GetRoomIdFromDevice(messageObject.DeviceId);
        
        // Sends the latest device readings to any active clients subscribed to the device
        var subscribedUserList = StateService.GetUsersForRoom(roomId);

        if (subscribedUserList.Count == 0)
            return; // No subscribed listeners, stop here.

        var endTime = DateTime.Now.ToLocalTime();
        var startTime = endTime - TimeSpan.FromMinutes(24 * 60); // Last 24 hours so it matches with chart data
        
        var temperatureReadings = _roomReadingsService.GetTemperatureReadingsFromRoom
            (roomId, startTime, endTime, 120);
        
        var humidityReadings = _roomReadingsService.GetHumidityReadingsFromRoom
            (roomId, startTime, endTime, 120);
        
        var pm25Readings = _roomReadingsService.GetPm25ReadingsFromRoom
            (roomId, startTime, endTime, 120);
        
        var pm100Readings = _roomReadingsService.GetPm100ReadingsFromRoom
            (roomId, startTime, endTime, 120);
        
        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetClient(user);
            if (!ReferenceEquals(connection, null))
            {
                connection.Connection.SendDto(new ServerSendsTemperatureReadingsForRoom()
                {
                    RoomId = roomId,
                    TemperatureReadings = temperatureReadings
                });
                
                connection.Connection.SendDto(new ServerSendsHumidityReadingsForRoom()
                {
                    RoomId = roomId,
                    HumidityReadings = humidityReadings
                });
                
                connection.Connection.SendDto(new ServerSendsPm25ReadingsForRoom()
                {
                    RoomId = roomId,
                    Pm25Readings = pm25Readings
                });
                
                connection.Connection.SendDto(new ServerSendsPm100ReadingsForRoom()
                {
                    RoomId = roomId,
                    Pm100Readings = pm100Readings
                });
            }
        }
    }
    
    private void SendLatestRoomReadingsToClient(int deviceId)
    {
        var roomId = _deviceService.GetRoomIdFromDevice(deviceId);
        
        // Sends the latest room readings to any active clients subscribed to the room
        var subscribedUserList = StateService.GetUsersForRoom(roomId);

        if (subscribedUserList.Count == 0)
            return; // No subscribed listeners, stop here.

        // Get the latest averaged readings from the room
        var latestReadings = _roomReadingsService.GetLatestReadingsFromRoom(roomId);

        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetClient(user);
            if (connection != null)
            {
                connection.Connection.SendDto(new ServerSendsLatestRoomReadings()
                {
                    Data = latestReadings
                });
            }
        }
    }
}


