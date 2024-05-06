import {inject, Inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {
  ClientWantsAllRooms,
  ClientWantsSpecificRoom,
  ClientWantsToCreateRoom,
  ClientWantsToDeleteRoom, ClientWantsToEditRoom
} from "../../../models/clientRequests";
import {Room} from "../../../models/room";

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private ws: WebSocketConnectionService = inject(WebSocketConnectionService);

  constructor() {
  }


  createRoom(roomname: string) {
    this.ws.socketConnection.sendDto(
      new ClientWantsToCreateRoom({
          RoomName: roomname
        }
      )
    )
  }

  getAllRooms() {
    this.ws.socketConnection.sendDto(
      new ClientWantsAllRooms({
      })
    )
  }
  getAllRoomRecords(){
    return this.ws.allRooms
  }

  getRoomById(idFromRoute: number) {
    this.ws.socketConnection.sendDto(
      new ClientWantsSpecificRoom({
        RoomId: idFromRoute
      })
    )
  }

  deleteRoom(Id: number) {
    this.ws.socketConnection.sendDto(
      new ClientWantsToDeleteRoom({
        RoomId: Id
      })
    )
  }

  editRoom(Roomname: string, Id: number) {
    this.ws.socketConnection.sendDto(
      new ClientWantsToEditRoom({
        RoomId: Id,
        RoomName: Roomname
      })
    )
  }

  isRoomEdited() {
    return this.ws.isRoomEdited
  }

    updateRoom(room: Room) {
        //TODO
    }
}


