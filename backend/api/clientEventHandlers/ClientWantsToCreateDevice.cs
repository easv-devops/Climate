using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.mqttEventListeners;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToCreateDeviceDto : BaseDto
{
    [Required(ErrorMessage = "Device Name is required")]
    [MaxLength(50, ErrorMessage = "Device Name is too long")]
    public string DeviceName { get; set; }
    [Required(ErrorMessage = "Room Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Room Id is not a valid number")]
    public int RoomId { get; set; }
    public RangeDto? DeviceRange { get; set; }
    public SettingsDto? DeviceSettings { get; set; }
}


[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToCreateDevice : BaseEventHandler<ClientWantsToCreateDeviceDto>
    {
        private readonly DeviceService _deviceService;
        private readonly DeviceSettingsService _deviceSettingsService;
        private MqttClientSubscriber _mqttClientSubscriber;

        public ClientWantsToCreateDevice(DeviceService deviceService, DeviceSettingsService deviceSettingsService, MqttClientSubscriber mqttClientSubscriber)
        {
            _deviceService = deviceService;
            _deviceSettingsService = deviceSettingsService;
            _mqttClientSubscriber = mqttClientSubscriber;
        }


        public override Task Handle(ClientWantsToCreateDeviceDto dto, IWebSocketConnection socket)
        {
            var response = _deviceService.CreateDevice(new DeviceDto()
            {
                DeviceName = dto.DeviceName,
                RoomId = dto.RoomId
            });

            RangeDto rangeSettings;
            if (ReferenceEquals(dto.DeviceRange, null)) //sets the range if the users has not selected any
            {
                rangeSettings = _deviceSettingsService.CreateRangeSetting(new RangeDto
                {
                    Id = response.Id,
                    TemperatureMax = 25,
                    TemperatureMin = 15,
                    HumidityMax = 30,
                    HumidityMin = 60,
                    Particle25Max = 15,
                    Particle100Max = 15
                });
            }
            else
            {
                rangeSettings = _deviceSettingsService.CreateRangeSetting(dto.DeviceRange);
            }
        
            
            StateService.AddDeviceSettings(rangeSettings);//adds the range settings to the state service 

            SettingsDto deviceSettings;
            
            if (ReferenceEquals(dto.DeviceSettings, null)) //sets the settings if the users has not selected any
            {
                deviceSettings = new SettingsDto
                {
                    Id = response.Id,
                    BMP280ReadingInterval = 2,
                    PMSReadingInterval = 5,
                    UpdateInterval = 10
                };
                
            }
            else
            {
                deviceSettings = dto.DeviceSettings;
            }
            _deviceSettingsService.CreateDeviceSettings(deviceSettings);

            //sends the new settings to the device
            _mqttClientSubscriber.SendMessageToBroker(deviceSettings);


            //add user to state service for later use
            StateService.AddUserToDevice(response.Id, socket.ConnectionInfo.Id);
            
            socket.SendDto(new ServerSendsDevice
            {
                DeviceName = response.DeviceName,
                RoomId = response.RoomId,
                Id = response.Id
            });
            return Task.CompletedTask;
        }
    }


public class ServerSendsDevice : BaseDto
{
    public required int Id { get; set; }
    public required string DeviceName { get; set; }
    public required int RoomId { get; set; }
}