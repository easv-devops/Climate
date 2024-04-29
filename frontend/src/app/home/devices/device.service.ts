import {Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToGetDeviceByIdDto} from "../../../models/ClientWantsToGetDeviceByIdDto";
import {ClientWantsToGetDevicesByRoomIdDto} from "../../../models/ClientWantsToGetDevicesByRoomIdDto";
import {ClientWantsToCreateDeviceDto} from "../../../models/ClientWantsToCreateDeviceDto";
import {ClientWantsToDeleteDeviceDto} from "../../../models/ClientWantsToDeleteDevice";

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

  getDeviceById(id: number){
    var dto = new ClientWantsToGetDeviceByIdDto({
      DeviceId: id
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getDeviceObservable() {
    return this.ws.device;
  }

  getDevicesByRoomId(id: number){
    var dto = new ClientWantsToGetDevicesByRoomIdDto({
      RoomId: id
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getRoomDevicesObservable(){
    return this.ws.roomDevices;
  }
}
