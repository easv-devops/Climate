import {Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToGetDeviceByIdDto} from "../../../models/ClientWantsToGetDeviceByIdDto";
import {ClientWantsToGetDevicesByRoomIdDto} from "../../../models/ClientWantsToGetDevicesByRoomIdDto";

@Injectable({providedIn: 'root'})
export class DeviceService {

  constructor(private ws: WebSocketConnectionService) {
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
