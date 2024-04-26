using api.helpers;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToCreateDeviceDto : BaseDto
{
    public string DeviceName { get; set; }
    public int RoomId { get; set; }
}



public class ClientWantsToCreateDevice : BaseEventHandler<ClientWantsToCreateDeviceDto>
    {

        private readonly DeviceService _deviceService;
        
        public ClientWantsToCreateDevice(DeviceService deviceService)
        {
            _deviceService = deviceService;
        }


        public override Task Handle(ClientWantsToCreateDeviceDto dto, IWebSocketConnection socket)
        {
            var response = _deviceService.CreateDevice(new DeviceDto()
            {
                DeviceName = dto.DeviceName,
                RoomId = dto.RoomId
            });
            
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