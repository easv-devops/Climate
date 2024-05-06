using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels.rooms;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToEditRoomDto : BaseDto
{
    [Required(ErrorMessage = "RoomId is required")]
    public int RoomId { get; set; }
    
    [Required(ErrorMessage = "RoomId is required")]
    [MinLength(2, ErrorMessage = "Room name is too short.")]
    public string RoomName { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToEditRoom : BaseEventHandler<ClientWantsToEditRoomDto>
{
    private readonly RoomService _roomService;
    public ClientWantsToEditRoom(RoomService roomService)
    {
        _roomService = roomService;
    }
    
    public override Task Handle(ClientWantsToEditRoomDto dto, IWebSocketConnection socket)
    {
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User!.Id;

        Room room = new Room()
        {
            Id = dto.RoomId,
            UserId = userId,
            RoomName = dto.RoomName
        };
        Room editRoom = _roomService.EditRoom(room);
        Room specificRoom = _roomService.getSpecificRoom(room);

        socket.SendDto(
            new ServerReturnsSpecificRoom()
            {
                specificRoom = specificRoom
            }
        );
        return Task.CompletedTask;
    }
}