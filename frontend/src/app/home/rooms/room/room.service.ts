import {inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {ClientWantsToGetAllRoomsDto} from "../../../../models/roomModels/clientWantsToGetAllRoomsDto";
import {ClientWantsToCreateRoomDto} from "../../../../models/roomModels/ClientWantsToCreateRoomDto";
import {ClientWantsToEditRoomDto} from "../../../../models/roomModels/ClientWantsToEditRoomDto";
import {ClientWantsToDeleteRoomDto} from "../../../../models/roomModels/ClientWantsToDeleteRoomDto";


@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private ws: WebSocketConnectionService = inject(WebSocketConnectionService);

  constructor() {
  }


  getAllRooms() {
    this.ws.socketConnection.sendDto(
      new ClientWantsToGetAllRoomsDto({
      })
    )
  }

  //todo send et rigtigt objekt med, men den virker
  createRoom(name: string){
    this.ws.socketConnection.sendDto(new ClientWantsToCreateRoomDto({
      RoomToCreate: {
        RoomName: name
      }
    }));
  }

  editRoom(roomId: number, name: string){
    this.ws.socketConnection.sendDto(new ClientWantsToEditRoomDto( new ClientWantsToEditRoomDto({RoomToEdit: {
        Id: roomId as number,
        RoomName: name
      }})
    ));
  }

  deleteRoom(roomId: number){
    this.ws.socketConnection.sendDto(new ClientWantsToDeleteRoomDto(
      {RoomToDelete: roomId}
    ));
  }
}

