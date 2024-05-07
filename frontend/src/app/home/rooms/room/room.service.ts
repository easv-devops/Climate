import {inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {ClientWantsToGetAllRoomsDto} from "../../../../models/roomModels/clientWantsToGetAllRoomsDto";
import {ClientWantsToCreateRoomDto} from "../../../../models/roomModels/ClientWantsToCreateRoomDto";


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
  createRoom(){
    this.ws.socketConnection.sendDto(new ClientWantsToCreateRoomDto({
      RoomToCreate: {
        UserId: 1, // Erstat med den ønskede bruger-ID
        RoomName: "Living Room" // Erstat med det ønskede navn på rummet
      }
    }));
  }

}

