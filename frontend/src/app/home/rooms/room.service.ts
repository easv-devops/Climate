import {inject, Inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsAllRooms, ClientWantsToCreateRoom} from "../../../models/clientRequests";
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
    return this.ws.allRoomsRecord
  }
}


