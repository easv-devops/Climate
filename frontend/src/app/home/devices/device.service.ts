import {Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToCreateDeviceDto} from "../../../models/ClientWantsToCreateDeviceDto";

import {ClientWantsToEditDeviceDto} from "../../../models/ClientWantsToEditDeviceDto";
import {Device} from "../../../models/Entities";

import {ClientWantsToDeleteDeviceDto} from "../../../models/ClientWantsToDeleteDevice";
import {ClientWantsToGetDeviceIdsForRoomDto} from "../../../models/ClientWantsToGetDeviceIdsForRoomDto";
import {ClientWantsToGetTemperatureReadingsDto} from "../../../models/ClientWantsToGetTemperatureReadingsDto";
import {ClientWantsToGetHumidityReadingsDto} from "../../../models/ClientWantsToGetHumidityReadings";
import {ClientWantsToGetPm25ReadingsDto} from "../../../models/ClientWantsToGetPm25ReadingsDto";
import {ClientWantsToGetPm100ReadingsDto} from "../../../models/ClientWantsToGetPm100ReadingsDto";


@Injectable({providedIn: 'root'})
export class DeviceService {

  constructor(private ws: WebSocketConnectionService) {
  }

  createDevice(createDeviceDto: ClientWantsToCreateDeviceDto) {
    this.ws.socketConnection.sendDto(createDeviceDto)
  }

  deleteDevice(deleteDeviceDto: ClientWantsToDeleteDeviceDto) {
    this.ws.socketConnection.sendDto(deleteDeviceDto)
  }

  editDevice(dto: ClientWantsToEditDeviceDto){
    this.ws.socketConnection.sendDto(dto)
  }

  isDeviceEdited() {
    return this.ws.isDeviceEdited
  }

  getTemperatureByDeviceId(id: number) {
    var dto = new ClientWantsToGetTemperatureReadingsDto({
      DeviceId: id
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getHumidityByDeviceId(id: number) {
    var dto = new ClientWantsToGetHumidityReadingsDto({
      DeviceId: id
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getPm25ByDeviceId(id: number) {
    var dto = new ClientWantsToGetPm25ReadingsDto({
      DeviceId: id
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getPm100ByDeviceId(id: number) {
    var dto = new ClientWantsToGetPm100ReadingsDto({
      DeviceId: id
    });
    this.ws.socketConnection.sendDto(dto)
  }
}
