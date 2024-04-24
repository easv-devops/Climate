import {inject, Inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../web-socket-connection.service";
import {
    ClientWantsToCreateDeviceDto
} from "../../models/ClientWantsToCreateDeviceDto";

@Injectable({
    providedIn: 'root'
})

export class DeviceService{
    private ws: WebSocketConnectionService = inject(WebSocketConnectionService);


    constructor() {
    }

    createDevice(createDeviceDto: ClientWantsToCreateDeviceDto){
        this.ws.socketConnection.sendDto(createDeviceDto)
    }
}
