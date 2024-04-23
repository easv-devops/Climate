using System.Text.Json;
using api.helpers;
using Fleck;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;

namespace api.mqttEventListeners;

public class MqttClientSubscriber
{
    
    public async Task CommunicateWithBroker()
    {

        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();
        
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("mqtt.flespi.io", 1883)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .WithCredentials(Environment.GetEnvironmentVariable(EnvVarKeys.MqttToken.ToString()), "") // todo should be a secret
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic("Climate"))
            .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var message = e.ApplicationMessage.ConvertPayloadToString();
                var messageObject = JsonSerializer.Deserialize<MqttClientWantsToSendMessageToRoom>(message);
                var timestamp = DateTimeOffset.UtcNow;

                //todo save readings to db
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
}

public class MqttClientWantsToSendMessageToRoom
{
    public int DeviceId { get; set; }
    public Object DataObject { get; set; } //should be 
}