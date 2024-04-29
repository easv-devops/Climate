import {Injectable} from "@angular/core";
import {environment} from "../environments/environment";
import {WebsocketSuperclass} from "../models/websocketSuperclass";
import {BaseDto} from "../models/baseDto";
import {
  ServerAuthenticatesUserDto,
  ServerRegisterUserDto,
  ServerResetsPasswordDto,
  ServerSendsErrorMessageToClient,
  DeviceWithIdDto
} from "../models/returnedObjectsFromBackend";
import {BehaviorSubject, Observable, take} from "rxjs";
import {ErrorHandlingService} from "./error-handling.service";
import {Device, DeviceInRoom} from "../models/Entities";
import {ServerSendsDeviceByIdDto} from "../models/ServerSendsDeviceByIdDto";
import {ServerSendsDevicesByUserIdDto} from "../models/ServerSendsDevicesByUserIdDto";
import {ServerSendsDevicesByRoomIdDto} from "../models/ServerSendsDevicesByRoomIdDto";
import {ServerEditsDeviceDto} from "../models/ServerEditsDeviceDto";


@Injectable({providedIn: 'root'})
export class WebSocketConnectionService {



  //Different objects used in the application
  //TODO check for these objects. Make sure they are used or removed
  //TODO use records instead of lists
  //todo should be objects instead of number, reference to the object (key= id. value = object)
  //todo maybe not "AllDevices" but just "devices". We should lazy load with longer json elements.
  AllRooms: number[] = [];

  //todo we should maybe have an endpoint for getting a user we can call when hitting the main page

  //observable jwt  --remember to unsub when done using (se login JWT ngOnit for more info)
  private jwtSubject = new BehaviorSubject<string | undefined>(undefined);
  jwt: Observable<string | undefined> = this.jwtSubject.asObservable();

  //used to reset password
  private isResetSubject = new BehaviorSubject<boolean | undefined>(undefined);
  isReset: Observable<boolean | undefined> = this.isResetSubject.asObservable();

  //Socket connection
  public socketConnection: WebsocketSuperclass;

  private deviceSubject = new BehaviorSubject<Device | undefined>(undefined);
  device: Observable<Device | undefined> = this.deviceSubject.asObservable();

  private deviceIdSubject = new BehaviorSubject<number | undefined>(undefined);
  deviceId: Observable<number | undefined> = this.deviceIdSubject.asObservable();

  private allDevicesSubject = new BehaviorSubject<Record<number, Device> | undefined>(undefined);
  allDevices: Observable<Record<number, Device> | undefined> = this.allDevicesSubject.asObservable();

  private roomDevicesSubject = new BehaviorSubject<DeviceInRoom[] | undefined>(undefined);
  roomDevices: Observable<DeviceInRoom[] | undefined> = this.roomDevicesSubject.asObservable();

  private isDeviceEditedSubject = new BehaviorSubject<boolean | undefined>(undefined);
  isDeviceEdited: Observable<boolean | undefined> = this.isDeviceEditedSubject.asObservable();

  constructor(private errorHandlingService: ErrorHandlingService) {
    //Pointing to the direction the websocket can be found at
    this.socketConnection = new WebsocketSuperclass(environment.websocketBaseUrl);
    this.handleEvent();
  }

  handleEvent() {
    this.socketConnection.onmessage = (event) => {
      const data = JSON.parse(event.data) as BaseDto<any>;
      // @ts-ignore
      this[data.eventType].call(this,data);
    }
  }

  //All the return objects from the webSocket
  //These methods are triggered from the responses from the backend
  ServerAuthenticatesUser(dto: ServerAuthenticatesUserDto) {
    this.jwtSubject.next(dto.Jwt);
  }

  ServerSendsErrorMessageToClient(dto: ServerSendsErrorMessageToClient) {
    const errorMessage = 'Something went wrong. ' + dto.errorMessage;
    this.errorHandlingService.handleError(errorMessage);
  }

  ServerRegisterUser(dto: ServerRegisterUserDto) {
    this.jwtSubject.next(dto.Jwt);
  }

  ServerResetsPassword(dto: ServerResetsPasswordDto) {
    this.isResetSubject.next(dto.IsReset);
  }

  ServerSendsDeviceById(dto: ServerSendsDeviceByIdDto){
    this.deviceSubject.next(dto.Device)
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

  ServerSendsDevicesByRoomId(dto: ServerSendsDevicesByRoomIdDto){
    this.roomDevicesSubject.next(dto.Devices)
  }

  ServerSendsDevice(dto: DeviceWithIdDto) {
    this.allDevices.pipe(take(1)).subscribe(allDevicesRecord => {
      if (allDevicesRecord !== undefined) {
        allDevicesRecord[dto.Id] = dto;
        // Opdater allDevicesSubject med den opdaterede liste over enheder
        this.allDevicesSubject.next(allDevicesRecord);
      }
    });
  }

  ServerEditsDevice(dto: ServerEditsDeviceDto) {
    this.isDeviceEditedSubject.next(dto.IsEdit)
  }
}

