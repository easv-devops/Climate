import {Injectable} from '@angular/core';

import {WebSocketConnectionService} from "../web-socket-connection.service";
import {ClientWantsToCreateDeviceDto} from "../../models/ClientWantsToCreateDeviceDto";

@Injectable({
    providedIn: 'root'
})
export class DeviceService {

    constructor(
        public ws: WebSocketConnectionService
    ){

    }

    createDevice(createDeviceDto: ClientWantsToCreateDeviceDto) {
        this.ws.socketConnection.sendDto(createDeviceDto)
    }
}
