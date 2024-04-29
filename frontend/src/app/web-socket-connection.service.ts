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
import {BehaviorSubject, Observable} from "rxjs";
import {ErrorHandlingService} from "./error-handling.service";
import {Device, DeviceInRoom} from "../models/Entities";
import {ServerSendsDeviceByIdDto} from "../models/ServerSendsDeviceByIdDto";
import {ServerSendsDevicesByUserIdDto} from "../models/ServerSendsDevicesByUserIdDto";
import {ServerSendsDevicesByRoomIdDto} from "../models/ServerSendsDevicesByRoomIdDto";


@Injectable({providedIn: 'root'})
export class WebSocketConnectionService {

  //Different objects used in the application
  //TODO check for these objects. Make sure they are used or removed
  //TODO use records instead of lists
  //todo should be objects instead of number, reference to the object (key= id. value = object)
  //todo maybe not "AllDevices" but just "devices". We should lazy load with longer json elements.
  AllRooms: number[] = [];
  AllDevices: number[] = [];
  //todo we should maybe have an endpoint for getting a user we can call when hitting the main page

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

  private deviceSubject = new BehaviorSubject<Device | undefined>(undefined);
  device: Observable<Device | undefined> = this.deviceSubject.asObservable();

  private deviceIdSubject = new BehaviorSubject<number | undefined>(undefined);
  deviceId: Observable<number | undefined> = this.deviceIdSubject.asObservable();

  private allDevicesSubject = new BehaviorSubject<Device[] | undefined>(undefined);
  allDevices: Observable<Device[] | undefined> = this.allDevicesSubject.asObservable();

  private roomDevicesSubject = new BehaviorSubject<DeviceInRoom[] | undefined>(undefined);
  roomDevices: Observable<DeviceInRoom[] | undefined> = this.roomDevicesSubject.asObservable();

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

  ServerSendsDevicesByUserId(dto: ServerSendsDevicesByUserIdDto){
    this.allDevicesSubject.next(dto.Devices)
  }

  ServerSendsDevicesByRoomId(dto: ServerSendsDevicesByRoomIdDto){
    this.roomDevicesSubject.next(dto.Devices)
  }

  ServerSendsDevice(dto: DeviceWithIdDto){
    this.deviceIdSubject.next(dto.Id);
  }

  ServerSendsDeviceDeletionStatus(dto: ServerSendsDeviceDeletionStatusDto) {
    this.isDeletedSubject.next(dto.IsDeleted);
  }
}


