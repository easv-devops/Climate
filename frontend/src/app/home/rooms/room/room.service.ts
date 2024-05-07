import {inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {ClientWantsToGetAllRoomsDto} from "../../../../models/roomModels/clientWantsToGetAllRoomsDto";
import {ClientWantsToCreateRoomDto} from "../../../../models/roomModels/ClientWantsToCreateRoomDto";
import {ClientWantsToEditRoomDto} from "../../../../models/roomModels/ClientWantsToEditRoomDto";


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

  editRoom(){
    this.ws.socketConnection.sendDto(new ClientWantsToEditRoomDto( new ClientWantsToEditRoomDto({RoomToEdit: {
        Id: 1, // Id på det rum, du vil redigere
        RoomName: 'New Room Name' // Det nye navn til rummet
      }})
    ));
  }
}

