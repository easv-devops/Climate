using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using api.helpers;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers.roomClientHandlers;

public class ClientWantsToGetPm100ReadingsForRoomDto : BaseDto
{
    [Required(ErrorMessage = "Room Id is required")]
    public int RoomId { get; set; }
    
    public DateTime? StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
    
    public int Interval { get; set; }
}

public class ClientWantsToGetPm100ReadingsForRoom : BaseEventHandler<ClientWantsToGetPm100ReadingsForRoomDto>
{
    private readonly RoomReadingsService _roomReadingsService;
    
    public ClientWantsToGetPm100ReadingsForRoom(RoomReadingsService roomReadingsService)
    {
        _roomReadingsService = roomReadingsService;
    }

    public override Task Handle(ClientWantsToGetPm100ReadingsForRoomDto dto, IWebSocketConnection socket)
    {
        var guid = socket.ConnectionInfo.Id;

        if (!StateService.UserHasRoom(guid, dto.RoomId))
        {
            throw new AuthenticationException("Only the owner of room #" + dto.RoomId + " has access to this information");
        }

        var readings = _roomReadingsService.GetPm100ReadingsFromRoom(dto.RoomId, dto.StartTime, dto.EndTime, dto.Interval);

        socket.SendDto(new ServerSendsPm100ReadingsForRoom
        {
            RoomId = dto.RoomId,
            Readings = readings
        });

        return Task.CompletedTask;
    }
}

public class ServerSendsPm100ReadingsForRoom: BaseDto, IRoomReadingsDto
{
    public int RoomId { get; set; }
    public IEnumerable<SensorDto> Readings { get; set; }
}