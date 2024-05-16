import {inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToGetAllRoomsDto} from "../../../models/roomModels/clientWantsToGetAllRoomsDto";
import {ClientWantsToCreateRoomDto} from "../../../models/roomModels/ClientWantsToCreateRoomDto";
import {ClientWantsToEditRoomDto} from "../../../models/roomModels/ClientWantsToEditRoomDto";
import {ClientWantsToDeleteRoomDto} from "../../../models/roomModels/ClientWantsToDeleteRoomDto";
import {ClientWantsToGetTemperatureReadingsDto} from "../../../models/ClientWantsToGetTemperatureReadingsDto";
import {ClientWantsToGetHumidityReadingsDto} from "../../../models/ClientWantsToGetHumidityReadings";
import {ClientWantsToGetPm25ReadingsDto} from "../../../models/ClientWantsToGetPm25ReadingsDto";
import {ClientWantsToGetPm100ReadingsDto} from "../../../models/ClientWantsToGetPm100ReadingsDto";
import {
  ClientWantsToGetTemperatureReadingsForRoomDto
} from "../../../models/roomModels/roomReadingModels/ClientWantsToGetTemperatureReadingsForRoomDto";
import {
  ClientWantsToGetHumidityReadingsForRoomDto
} from "../../../models/roomModels/roomReadingModels/ClientWantsToGetHumidityReadingsForRoomDto";
import {
  ClientWantsToGetPm25ReadingsForRoomDto
} from "../../../models/roomModels/roomReadingModels/ClientWantsToGetPm25ReadingsForRoomDto";
import {
  ClientWantsToGetPm100ReadingsForRoomDto
} from "../../../models/roomModels/roomReadingModels/ClientWantsToGetPm100ReadingsForRoomDto";


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


  getTemperatureByRoomId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetTemperatureReadingsForRoomDto({
      RoomId: id,
      StartTime: start,
      EndTime: end,
      Interval: 120,
    });

    this.ws.socketConnection.sendDto(dto)
  }

  getHumidityByRoomId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetHumidityReadingsForRoomDto({
      RoomId: id,
      StartTime: start,
      EndTime: end
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getPm25ByRoomId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetPm25ReadingsForRoomDto({
      RoomId: id,
      StartTime: start,
      EndTime: end
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getPm100ByRoomId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetPm100ReadingsForRoomDto({
      RoomId: id,
      StartTime: start,
      EndTime: end
    });
    this.ws.socketConnection.sendDto(dto)
  }
}
