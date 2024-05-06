using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToDeleteRoomDto : BaseDto
{
    
    [Required(ErrorMessage = "RoomId is required")]
    public int RoomId { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToDeleteRoom : BaseEventHandler<ClientWantsToDeleteRoomDto>
{
    private readonly RoomService _roomService;

    public ClientWantsToDeleteRoom(RoomService roomService)
    {
        _roomService = roomService;
    }
    
    public override Task Handle(ClientWantsToDeleteRoomDto dto, IWebSocketConnection socket)
    {
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User!.Id;

        Room room = new Room
        {
            Id = dto.RoomId,
            UserId = userId
        };
        _roomService.DeleteRoom(room);
        List<Room> allrooms = _roomService.GetAllRooms(userId);
        socket.SendDto(
            new ServerReturnsAllRooms()
            {
                rooms = allrooms
            }
        );
        return Task.CompletedTask;
    }
}