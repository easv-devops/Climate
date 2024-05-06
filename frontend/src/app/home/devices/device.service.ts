import {Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToCreateDeviceDto} from "../../../models/ClientWantsToCreateDeviceDto";

import {ClientWantsToEditDeviceDto} from "../../../models/ClientWantsToEditDeviceDto";
import {Device} from "../../../models/Entities";

import {ClientWantsToDeleteDeviceDto} from "../../../models/ClientWantsToDeleteDevice";
import {ClientWantsToGetDeviceIdsForRoomDto} from "../../../models/ClientWantsToGetDeviceIdsForRoomDto";


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


  getDevicesByRoomId(id: number){
    var dto = new ClientWantsToGetDeviceIdsForRoomDto({
      RoomId: id
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getRoomDevicesObservable(){
    return this.ws.roomDevices;
  }

  editDevice(dto: ClientWantsToEditDeviceDto){
    this.ws.socketConnection.sendDto(dto)
  }

  isDeviceEdited() {
    return this.ws.isDeviceEdited
  }

  updateDevice(device: Device) {
    //TODO
  }
}
