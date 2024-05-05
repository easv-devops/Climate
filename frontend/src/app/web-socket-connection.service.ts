import {Injectable} from "@angular/core";
import {environment} from "../environments/environment";
import {WebsocketSuperclass} from "../models/websocketSuperclass";
import {BaseDto} from "../models/baseDto";
import {
  DeviceWithIdDto,
  ServerAuthenticatesUserDto,
  ServerRegisterUserDto,
  ServerResetsPasswordDto,
  ServerSendsDeviceDeletionStatusDto,
  ServerSendsErrorMessageToClient
} from "../models/returnedObjectsFromBackend";
import {BehaviorSubject, Observable, take} from "rxjs";
import {ErrorHandlingService} from "./error-handling.service";
import {Device, DeviceInRoom, SensorDto} from "../models/Entities";
import {ServerSendsDevicesByRoomIdDto} from "../models/ServerSendsDevicesByRoomIdDto";
import {ServerEditsDeviceDto} from "../models/ServerEditsDeviceDto";
import {ServerSendsDevicesByUserIdDto} from "../models/ServerSendsDevicesByUserIdDto";
import {ClientWantsToGetDevicesByUserIdDto} from "../models/ClientWantsToGetDevicesByUserIdDto";
import {ServerSendsTemperatureReadingsDto} from "../models/ServerSendsTemperatureReadingsDto";
import {ServerSendsHumidityReadingsDto} from "../models/ServerSendsHumidityReadingsDto";
import {ServerSendsPm25ReadingsDto} from "../models/ServerSendsPm25ReadingsDto";
import {ServerSendsPm100ReadingsDto} from "../models/ServerSendsPm100ReadingsDto";


@Injectable({providedIn: 'root'})
export class WebSocketConnectionService {

  //Different objects used in the application
  //TODO check for these objects. Make sure they are used or removed
  //todo we should maybe have an endpoint for getting a user we can call when hitting the main page
  AllRooms: number[] = [];
  //observable jwt  --remember to unsub when done using (se login JWT ngOnit for more info)
  private jwtSubject = new BehaviorSubject<string | undefined>(undefined);
  jwt: Observable<string | undefined> = this.jwtSubject.asObservable();

  //used to reset password
  private isResetSubject = new BehaviorSubject<boolean | undefined>(undefined);
  isReset: Observable<boolean | undefined> = this.isResetSubject.asObservable();

  private isDeletedSubject = new BehaviorSubject<boolean | undefined>(undefined);
  isDeleted: Observable<boolean | undefined> = this.isDeletedSubject.asObservable();

  //Socket connection
  public socketConnection: WebsocketSuperclass;

  //used for creating a new device (waits for the new returned device id)
  private deviceIdSubject = new BehaviorSubject<number | undefined>(undefined);
  deviceId: Observable<number | undefined> = this.deviceIdSubject.asObservable();

  private allDevicesSubject = new BehaviorSubject<Record<number, Device> | undefined>(undefined);
  allDevices: Observable<Record<number, Device> | undefined> = this.allDevicesSubject.asObservable();

  private roomDevicesSubject = new BehaviorSubject<DeviceInRoom[] | undefined>(undefined);
  roomDevices: Observable<DeviceInRoom[] | undefined> = this.roomDevicesSubject.asObservable();

  private isDeviceEditedSubject = new BehaviorSubject<boolean | undefined>(undefined);
  isDeviceEdited: Observable<boolean | undefined> = this.isDeviceEditedSubject.asObservable();

  private temperatureReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  temperatureReadings: Observable<Record<number, SensorDto[]> | undefined> = this.temperatureReadingsSubject.asObservable();

  private humidityReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  humidityReadings: Observable<Record<number, SensorDto[]> | undefined> = this.humidityReadingsSubject.asObservable();

  private pm25ReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  pm25Readings: Observable<Record<number, SensorDto[]> | undefined> = this.pm25ReadingsSubject.asObservable();

  private pm100ReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  pm100Readings: Observable<Record<number, SensorDto[]> | undefined> = this.pm100ReadingsSubject.asObservable();

  constructor(private errorHandlingService: ErrorHandlingService) {
    //Pointing to the direction the websocket can be found at
    this.socketConnection = new WebsocketSuperclass(environment.websocketBaseUrl);
    this.handleEvent();
  }

  handleEvent() {
    this.socketConnection.onmessage = (event) => {
      const data = JSON.parse(event.data) as BaseDto<any>;
      // @ts-ignore
      this[data.eventType].call(this, data);
    }
  }

  //All the return objects from the webSocket
  //These methods are triggered from the responses from the backend
  ServerAuthenticatesUser(dto: ServerAuthenticatesUserDto) {
    localStorage.setItem("jwt", dto.Jwt!);
    this.jwtSubject.next(dto.Jwt);
    this.socketConnection.sendDto(new ClientWantsToGetDevicesByUserIdDto({}));
  }

  ServerSendsErrorMessageToClient(dto: ServerSendsErrorMessageToClient) {
    const errorMessage = 'Something went wrong. ' + dto.errorMessage;
    this.errorHandlingService.handleError(errorMessage);
  }

  ServerRegisterUser(dto: ServerRegisterUserDto) {
    localStorage.setItem("jwt", dto.Jwt!);
    this.jwtSubject.next(dto.Jwt);
    this.socketConnection.sendDto(new ClientWantsToGetDevicesByUserIdDto({}));
  }

  ServerResetsPassword(dto: ServerResetsPasswordDto) {
    this.isResetSubject.next(dto.IsReset);
  }

  ServerSendsDevicesByRoomId(dto: ServerSendsDevicesByRoomIdDto) {
    this.roomDevicesSubject.next(dto.Devices)
  }

  ServerSendsDevice(dto: DeviceWithIdDto) {
    this.allDevices.pipe(take(1)).subscribe(allDevicesRecord => {
      if (allDevicesRecord !== undefined) {

        if (!allDevicesRecord.hasOwnProperty(dto.Id)) {
          this.deviceIdSubject.next(dto.Id);//if it is a new device the device id is set so create can find it
        }

        allDevicesRecord[dto.Id] = dto;
        // Opdater allDevicesSubject med den opdaterede liste over enheder
        this.allDevicesSubject.next(allDevicesRecord);
      }
    });
  }


  //todo could maybe just check if the new values has been changed on the alldevice list just maybe..
  ServerEditsDevice(dto: ServerEditsDeviceDto) {
    this.isDeviceEditedSubject.next(dto.IsEdit)
  }

  ServerSendsDevicesByUserId(dto: ServerSendsDevicesByUserIdDto) {

    this.allDevices.pipe(take(1)).subscribe(allDevicesRecord => {

      if (!allDevicesRecord) {
        allDevicesRecord = {};
      }

      dto.Devices!.forEach(device => {
        // Tilføj eller opdater enheden i record
        allDevicesRecord![device.Id] = device;
      });

      // Opdater allDevicesSubject med den opdaterede record
      this.allDevicesSubject.next(allDevicesRecord);
    });
  }

  //used to set when the client has handled edit
  setIsDeviceEdited(value: boolean) {
    this.isDeviceEditedSubject.next(value);
  }


  ServerSendsDeviceDeletionStatus(dto: ServerSendsDeviceDeletionStatusDto) {
    if (dto.IsDeleted && this.allDevicesSubject.value) {
      const devices = {...this.allDevicesSubject.value};
      delete devices[dto.Id];
      this.allDevicesSubject.next(devices);
    }
  }


  clearDataOnLogout() {
    localStorage.setItem("jwt", ""); // Nulstil JWT i local storage
    this.jwtSubject.next(undefined); // Nulstil JWT-subjektet
    this.isResetSubject.next(undefined); // Nulstil isReset-subjektet
    this.isDeletedSubject.next(undefined); // Nulstil isDeleted-subjektet
    this.deviceIdSubject.next(undefined); // Nulstil deviceId-subjektet
    this.allDevicesSubject.next(undefined); // Nulstil allDevices-subjektet
    this.roomDevicesSubject.next(undefined); // Nulstil roomDevices-subjektet
    this.isDeviceEditedSubject.next(undefined); // Nulstil isDeviceEdited-subjektet
  }

  ServerSendsTemperatureReadings(dto: ServerSendsTemperatureReadingsDto) {
    this.temperatureReadings.pipe(take(1)).subscribe(temperatureReadingsRecord => {

      if (!temperatureReadingsRecord) {
        temperatureReadingsRecord = {};
      }

      temperatureReadingsRecord![dto.DeviceId] = dto.TemperatureReadings;

      // Opdater temperatureReadingsSubject med den opdaterede record
      this.temperatureReadingsSubject.next(temperatureReadingsRecord);
    });
  }

  ServerSendsHumidityReadings(dto: ServerSendsHumidityReadingsDto) {
    this.humidityReadings.pipe(take(1)).subscribe(humidityReadingsRecord => {

      if (!humidityReadingsRecord) {
        humidityReadingsRecord = {};
      }

      humidityReadingsRecord![dto.DeviceId] = dto.HumidityReadings;

      // Opdater humidityReadingsSubject med den opdaterede record
      this.humidityReadingsSubject.next(humidityReadingsRecord);
    });
  }

  ServerSendsPm25Readings(dto: ServerSendsPm25ReadingsDto) {
    this.pm25Readings.pipe(take(1)).subscribe(pm25ReadingsRecord => {

      if (!pm25ReadingsRecord) {
        pm25ReadingsRecord = {};
      }

      pm25ReadingsRecord![dto.DeviceId] = dto.Pm25Readings;

      // Opdater pm25ReadingsSubject med den opdaterede record
      this.pm25ReadingsSubject.next(pm25ReadingsRecord);
    });
  }

  ServerSendsPm100Readings(dto: ServerSendsPm100ReadingsDto) {
    this.pm100Readings.pipe(take(1)).subscribe(pm100ReadingsRecord => {

      if (!pm100ReadingsRecord) {
        pm100ReadingsRecord = {};
      }

      pm100ReadingsRecord![dto.DeviceId] = dto.Pm100Readings;

      // Opdater pm100ReadingsSubject med den opdaterede record
      this.pm100ReadingsSubject.next(pm100ReadingsRecord);
    });
  }
}


