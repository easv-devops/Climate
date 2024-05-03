using api.helpers;
using api.serverEventModels;
using api.serverEventModels.rooms;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsSpecificRoomDto : BaseDto
{
    public int RoomId { get; set; }
}
public class ClientWantsSpecificRoom : BaseEventHandler<ClientWantsSpecificRoomDto>
{
    
    private readonly RoomService _roomService;

    public ClientWantsSpecificRoom(RoomService roomService)
    {
        _roomService = roomService;
    }

    public override Task Handle(ClientWantsSpecificRoomDto dto, IWebSocketConnection socket)
    {                                                                                                                                                                                                                                                                                                                                                                                                  
        var userId = StateService.GetClient(socket.ConnectionInfo.Id).User!.Id;

        Room room = new Room
        {
            Id = dto.RoomId,
            UserId = userId
        };

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