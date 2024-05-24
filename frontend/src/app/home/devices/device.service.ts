import {Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToCreateDeviceDto} from "../../../models/ClientWantsToCreateDeviceDto";

import {ClientWantsToEditDeviceDto} from "../../../models/ClientWantsToEditDeviceDto";

import {ClientWantsToDeleteDeviceDto} from "../../../models/ClientWantsToDeleteDevice";
import {ClientWantsToGetTemperatureReadingsDto} from "../../../models/ClientWantsToGetTemperatureReadingsDto";
import {ClientWantsToGetHumidityReadingsDto} from "../../../models/ClientWantsToGetHumidityReadings";
import {ClientWantsToGetPm25ReadingsDto} from "../../../models/ClientWantsToGetPm25ReadingsDto";
import {ClientWantsToGetPm100ReadingsDto} from "../../../models/ClientWantsToGetPm100ReadingsDto";
import {ClientWantsGetDeviceSettingsDto} from "../../../models/ClientWantsToGetDeviceSettingsDto";
import {DeviceRange, DeviceSettings, LatestData} from "../../../models/Entities";
import {ClientWantsToEditDeviceRangeDto} from "../../../models/ClientWantsToEditDeviceRange";
import {ClientWantsToEditDeviceSettingsDto} from "../../../models/ClientWantsToEditDeviceSettingsDto";
import {ClientWantsToGetLatestDeviceReadingsDto} from "../../../models/ClientWantsToGetLatestDeviceReadingsDto";
import {take} from "rxjs";

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

  getTemperatureByDeviceId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetTemperatureReadingsDto({
      DeviceId: id,
      StartTime: start,
      EndTime: end
    });

    this.ws.socketConnection.sendDto(dto)
  }

  getHumidityByDeviceId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetHumidityReadingsDto({
      DeviceId: id,
      StartTime: start,
      EndTime: end
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getPm25ByDeviceId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetPm25ReadingsDto({
      DeviceId: id,
      StartTime: start,
      EndTime: end
    });
    this.ws.socketConnection.sendDto(dto)
  }

  getPm100ByDeviceId(id: number, start: Date, end: Date) {
    var dto = new ClientWantsToGetPm100ReadingsDto({
      DeviceId: id,
      StartTime: start,
      EndTime: end
    });
    this.ws.socketConnection.sendDto(dto)
  }

  //gets all device settings for a given device
  getDeviceSettingsForDevice(deviceId: number){
    var dto = new ClientWantsGetDeviceSettingsDto({
      Id: deviceId
    })
    this.ws.socketConnection.sendDto(dto);
  }

  editDeviceRange(deviceRange: DeviceRange){
    var dto = new ClientWantsToEditDeviceRangeDto({
      Settings: deviceRange
    })
    this.ws.socketConnection.sendDto(dto);
  }

  updateDeviceSettings(updatedSettings: DeviceSettings) {
    var dto = new ClientWantsToEditDeviceSettingsDto({
      Settings: updatedSettings
    });
    this.ws.socketConnection.sendDto(dto);
  }

  getLatestReadings(deviceId: number): LatestData | undefined {
    this.ws.latestDeviceReadings
      .pipe(take(1))
      .subscribe(readings => {
        if (readings && readings[deviceId]) {
          // If our record already holds latest readings for this device, return that
          return readings[deviceId!];
        } else {
          // If not, request it from backend
          this.ws.socketConnection.sendDto(new ClientWantsToGetLatestDeviceReadingsDto({
            DeviceId: deviceId
          }))
          return undefined;
        }
      })
    return undefined;
  }
}
