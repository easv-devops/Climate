using System.Text.Json;
using api.clientEventHandlers.roomClientHandlers;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure;
using infrastructure.Models;
using lib;
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
      private readonly AlertService _alertService;

      private IMqttClient _mqttClient;
      private MqttFactory _mqttFactory;
      private MqttClientOptions _mqttClientOptions;

      public MqttClientSubscriber(DeviceReadingsService readingsService, RoomReadingsService roomReadingsService,
          DeviceService deviceService, AlertService alertService)
      {
          _readingsService = readingsService;
          _roomReadingsService = roomReadingsService;
          _deviceService = deviceService;
          _alertService = alertService;
          
 
          _mqttFactory = new MqttFactory();
          _mqttClient = _mqttFactory.CreateMqttClient();
      }
      
    
    public async Task ConnectToBroker()
    {
        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("mqtt.flespi.io", 1883)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .WithCredentials("FlespiToken iNAXKnfDnzPMgOqTfJkgYGOiYFdBDxhSdvH67RZK7r488rKRNG3EdqgFX9NYSW4T", "")
            .Build();

        await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic("Climate/#"))
            .Build();

        await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var message = e.ApplicationMessage.ConvertPayloadToString();

                var messageObject = JsonSerializer.Deserialize<DeviceData>(message);

                _readingsService.CreateReadings(messageObject);

                ScreenReadings(messageObject);
                
                SendDeviceReadingsToClient(messageObject);
                SendLatestDeviceReadingsToClient(messageObject);
                SendRoomReadingsToClient(messageObject);
                SendLatestRoomReadingsToClient(messageObject.DeviceId);

                
                _readingsService.CreateReadings(messageObject);
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
                SendDeviceReadings<ServerSendsTemperatureReadings>(connection, messageObject.DeviceId, messageObject.Data.Temperatures);
                SendDeviceReadings<ServerSendsHumidityReadings>(connection, messageObject.DeviceId, messageObject.Data.Humidities);
                SendDeviceReadings<ServerSendsPm25Readings>(connection, messageObject.DeviceId, messageObject.Data.Particles25);
                SendDeviceReadings<ServerSendsPm100Readings>(connection, messageObject.DeviceId, messageObject.Data.Particles100);
            }
        }
    }
    
    private void SendDeviceReadings<T>(WebSocketMetaData connection, int deviceId, IEnumerable<SensorDto> readings) 
        where T : BaseDto, IDeviceReadingsDto, new()
    {
        var dto = new T
        {
            DeviceId = deviceId,
            Readings = readings
        };
        connection.Connection.SendDto(dto);
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

        var roomReadings = _roomReadingsService.GetAllReadingsFromRoom(roomId);
        
        foreach (var user in subscribedUserList)
        {
            var connection = StateService.GetClient(user);
            if (!ReferenceEquals(connection, null))
            {
                SendRoomReadings<ServerSendsTemperatureReadingsForRoom>(connection, roomId, roomReadings.Temperatures);
                SendRoomReadings<ServerSendsHumidityReadingsForRoom>(connection, roomId, roomReadings.Humidities);
                SendRoomReadings<ServerSendsPm25ReadingsForRoom>(connection, roomId, roomReadings.Particles25);
                SendRoomReadings<ServerSendsPm100ReadingsForRoom>(connection, roomId, roomReadings.Particles100);
            }
        }
    }
    
    private void SendRoomReadings<T>(WebSocketMetaData connection, int roomId, IEnumerable<SensorDto> readings) 
        where T : BaseDto, IRoomReadingsDto, new()
    {
        var dto = new T
        {
            RoomId = roomId,
            Readings = readings
        };
        connection.Connection.SendDto(dto);
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

    private void ScreenReadings(DeviceData messageObject)
    {
        // Screens all readings for values out of range, and creates alerts in db
        var alerts = _alertService.ScreenReadings(messageObject);

        if (alerts.Count == 0)
            return; // No need to send an empty list

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

    public async Task SendMessageToBroker(SettingsDto settingsDto)
    {
        if (_mqttClient == null || !_mqttClient.IsConnected)
        {
            await ConnectToBroker();
        }
        
        var jsonPayload = JsonSerializer.Serialize(settingsDto);
        
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic("Climate/" + settingsDto.Id + "/settings")
            .WithPayload(jsonPayload)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .WithRetainFlag(false)
            .Build();
        await _mqttClient.PublishAsync(mqttMessage, CancellationToken.None);

    }
}
