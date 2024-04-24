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
            _deviceService.CreateDevice(new DeviceDto()
            {
                DeviceName = dto.DeviceName,
                RoomId = dto.RoomId
            });
            socket.SendDto(dto);
            return Task.CompletedTask;
        }


        

    
}