using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetLatestRoomReadingsDto : BaseDto
{
    [Required(ErrorMessage = "Room Id is required")]
    public int RoomId { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetLatestRoomReadings : BaseEventHandler<ClientWantsToGetLatestRoomReadingsDto>
{
    private readonly RoomReadingsService _roomReadingsService;
    
    public ClientWantsToGetLatestRoomReadings(RoomReadingsService roomReadingsService)
    {
        _roomReadingsService = roomReadingsService;
    }
    public override Task Handle(ClientWantsToGetLatestRoomReadingsDto dto, IWebSocketConnection socket)
    {
        var guid = socket.ConnectionInfo.Id;

        if (!StateService.UserHasRoom(guid, dto.RoomId))
        {
            throw new AuthenticationException("Only the owner of room #"+dto.RoomId+" has access to this information");
        }
        
        var data = _roomReadingsService.GetLatestReadingsFromRoom(dto.RoomId);
        
        socket.SendDto(new ServerSendsLatestRoomReadingsDto()
        {
            Data = data
        });

        return Task.CompletedTask;
    }
}