﻿using System.Text.Json;
using api.helpers;
using api.serverEventModels;
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
    private readonly AlertService _alertService;
    
    public MqttClientSubscriber(DeviceReadingsService readingsService, AlertService alertService)
    {
        _readingsService = readingsService;
        _alertService = alertService;
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

                ScreenReadings(messageObject);
                
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

    private void ScreenReadings(DeviceData messageObject)
    {
        // Screens all readings for values out of range, and creates alerts in db
        var alerts = _alertService.ScreenReadings(messageObject);
        
        // Sends all new alerts to any active clients subscribed to the device that sent readings
        var subscribedUserList = StateService.GetUsersForDevice(messageObject.DeviceId);

        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetClient(user);
            if (!ReferenceEquals(connection, null))
            {
                connection.Connection.SendDto(new ServerSendsAlertList()
                {
                    Alerts = alerts
                });
            }
        }
    }
}
