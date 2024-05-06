import {inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {ClientWantsToGetAllRoomsDto} from "../../../../models/roomModels/clientWantsToGetAllRoomsDto";


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
}

