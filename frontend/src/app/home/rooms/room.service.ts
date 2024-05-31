import {inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToGetAllRoomsDto} from "../../../models/roomModels/clientWantsToGetAllRoomsDto";
import {ClientWantsToCreateRoomDto} from "../../../models/roomModels/ClientWantsToCreateRoomDto";
import {ClientWantsToEditRoomDto} from "../../../models/roomModels/ClientWantsToEditRoomDto";
import {ClientWantsToDeleteRoomDto} from "../../../models/roomModels/ClientWantsToDeleteRoomDto";
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
import {LatestData} from "../../../models/Entities";
import {take} from "rxjs";
import {ClientWantsToGetLatestDeviceReadingsDto} from "../../../models/ClientWantsToGetLatestDeviceReadingsDto";
import {ClientWantsToGetLatestRoomReadingsDto} from "../../../models/ClientWantsToGetLatestRoomReadingsDto";


@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private ws: WebSocketConnectionService = inject(WebSocketConnectionService);

  constructor() {
  }

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
      Interval: 120
    });

    this.ws.socketConnection.sendDto(dto)
  }

  getHumidityByRoomId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetHumidityReadingsForRoomDto({
      RoomId: id,
      StartTime: start,
      EndTime: end,
      Interval: 120
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getPm25ByRoomId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetPm25ReadingsForRoomDto({
      RoomId: id,
      StartTime: start,
      EndTime: end,
      Interval: 120
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getPm100ByRoomId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetPm100ReadingsForRoomDto({
      RoomId: id,
      StartTime: start,
      EndTime: end,
      Interval: 120
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getLatestReadings(roomId: number): LatestData | undefined {
    this.ws.latestRoomReadings
      .pipe(take(1))
      .subscribe(readings => {
        if (readings && readings[roomId]) {
          // If our record already holds latest readings for this room, return that
          return readings[roomId!];
        } else {
          // If not, request it from backend
          this.ws.socketConnection.sendDto(new ClientWantsToGetLatestRoomReadingsDto({
            RoomId: roomId
          }))
          return undefined;
        }
      })
    return undefined;
  }
}
