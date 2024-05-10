using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.ServerEventHandlers;
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
}


[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToCreateDevice : BaseEventHandler<ClientWantsToCreateDeviceDto>
    {
        private readonly DeviceService _deviceService;
        private readonly ServerWantsToSendDevice _serverWantsToSendDevice;
        
        public ClientWantsToCreateDevice(DeviceService deviceService, ServerWantsToSendDevice serverWantsToSendDevice)
        {
            _deviceService = deviceService;
            _serverWantsToSendDevice = serverWantsToSendDevice;
        }


        public override Task Handle(ClientWantsToCreateDeviceDto dto, IWebSocketConnection socket)
        {
            var response = _deviceService.CreateDevice(new DeviceDto()
            {
                DeviceName = dto.DeviceName,
                RoomId = dto.RoomId
            });
            
            StateService.AddConnectionToDevice(response.Id, socket.ConnectionInfo.Id);
            
            _serverWantsToSendDevice.SendDeviceToClient(new DeviceWithIdDto
            {
                Id = response.Id,
                DeviceName = response.DeviceName,
                RoomId = response.RoomId
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