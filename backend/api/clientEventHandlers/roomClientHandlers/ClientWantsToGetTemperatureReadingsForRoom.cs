using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using api.helpers;
using api.serverEventModels;
using api.serverEventModels.roomDtos;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers.roomClientHandlers;

public class ClientWantsToGetTemperatureReadingsForRoomDto : BaseDto
{
    [Required(ErrorMessage = "Device Id is required")]
    public int RoomId { get; set; }
    
    public DateTime? StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
    
    public int Interval { get; set; }

}

public class ClientWantsToGetTemperatureReadingsForRoom: BaseEventHandler<ClientWantsToGetTemperatureReadingsForRoomDto>
{
    
    private readonly RoomReadingsService _roomReadingsService;
    public ClientWantsToGetTemperatureReadingsForRoom(RoomReadingsService roomReadingsService)
    {
        _roomReadingsService = roomReadingsService;
    }

    
    public override Task Handle(ClientWantsToGetTemperatureReadingsForRoomDto dto, IWebSocketConnection socket)
    {
        var guid = socket.ConnectionInfo.Id;

        if (!StateService.UserHasRoom(guid, dto.RoomId))
        {
            throw new AuthenticationException("Only the owner of room #"+dto.RoomId+" has access to this information");
        }
        
        
        
        
        var readings =
            _roomReadingsService.GetTemperatureReadingsFromRoom(dto.RoomId, dto.StartTime , dto.EndTime, dto.Interval);
        
        socket.SendDto(new ServerSendsTemperatureReadingsForRoom
        {
            RoomId = dto.RoomId,
            TemperatureReadings = readings
        });

        
        return Task.CompletedTask;
    }
}