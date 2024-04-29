import {inject, Inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToCreateRoom} from "../../../models/clientRequests";

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
}


