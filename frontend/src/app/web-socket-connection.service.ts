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
import {Device, DeviceInRoom, Room} from "../models/Entities";
import {ServerSendsDevicesByRoomIdDto} from "../models/ServerSendsDevicesByRoomIdDto";
import {ServerEditsDeviceDto} from "../models/ServerEditsDeviceDto";
import {ServerSendsDevicesByUserIdDto} from "../models/ServerSendsDevicesByUserIdDto";
import {ClientWantsToGetDevicesByUserIdDto} from "../models/ClientWantsToGetDevicesByUserIdDto";
import {ServerReturnsAllRoomsDto} from "../models/roomModels/ServerReturnsAllRoomsDto";
import {ClientWantsToGetAllRoomsDto} from "../models/roomModels/clientWantsToGetAllRoomsDto";
import {
  ServerSendsDeviceIdListForRoomDto
} from "../models/ServerSendsDeviceIdListForRoomDto";
import {ServerSendsRoom} from "../models/roomModels/ServerSendsRoom";
import { ServerDeletesRoom} from "../models/roomModels/ServerDeletesRoom";


@Injectable({providedIn: 'root'})
export class WebSocketConnectionService {

  //Different objects used in the application
  //TODO check for these objects. Make sure they are used or removed
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

  //used for creating a new device (waits for the new returned device id)
  private deviceIdSubject = new BehaviorSubject<number | undefined>(undefined);
  deviceId: Observable<number | undefined> = this.deviceIdSubject.asObservable();

  private allRoomsListSubject = new BehaviorSubject<number[] | undefined>(undefined);
  allRoomsList: Observable<number[] | undefined> = this.allRoomsListSubject.asObservable();


  private allDevicesSubject = new BehaviorSubject<Record<number, Device> | undefined>(undefined);
  allDevices: Observable<Record<number, Device> | undefined> = this.allDevicesSubject.asObservable();


  private allRoomsSubject = new BehaviorSubject<Record<number, Room> | undefined>(undefined);
  allRooms: Observable<Record<number, Room> | undefined> = this.allRoomsSubject.asObservable();

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
    localStorage.setItem("jwt", dto.Jwt!);
    this.jwtSubject.next(dto.Jwt);
    this.socketConnection.sendDto(new ClientWantsToGetDevicesByUserIdDto({}));
    this.socketConnection.sendDto(new ClientWantsToGetAllRoomsDto({}));
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

  ServerSendsDevicesByRoomId(dto: ServerSendsDevicesByRoomIdDto){
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

  ServerSendsRoom(dto: ServerSendsRoom) {
    this.allRooms.pipe(take(1)).subscribe(allRoomsRecord => {
      if (allRoomsRecord !== undefined) {

        allRoomsRecord[dto.Room.Id] = dto.Room;
        // Opdater allDevicesSubject med den opdaterede liste over enheder
        this.allRoomsSubject.next(allRoomsRecord);
        console.log(dto.Room)
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
      const devices = { ...this.allDevicesSubject.value };
      delete devices[dto.Id];
      this.allDevicesSubject.next(devices);
    }
  }

  ServerSendsDeviceIdListForRoom(dto: ServerSendsDeviceIdListForRoomDto) {

    //sets all rooms record
    this.allRooms.pipe(take(1)).subscribe(roomsSnapshot => {
      if (roomsSnapshot && roomsSnapshot[dto.RoomId]) {
        // Kopier det aktuelle rum
        const updatedRoom = { ...roomsSnapshot[dto.RoomId] };

        updatedRoom.DeviceIds = dto.DeviceIds;
        const updatedRoomsSnapshot = { ...roomsSnapshot, [dto.RoomId]: updatedRoom };
        // Udsend den opdaterede snapshot
        this.allRoomsSubject.next(updatedRoomsSnapshot);
      }
    });
  }

  ServerReturnsAllRooms(dto: ServerReturnsAllRoomsDto){
    var tempListOfRoomIds: number[] = [];
    this.allRooms.pipe(take(1)).subscribe(allRoomRecord => {
      if (!allRoomRecord) {
        allRoomRecord = {};
      }

      dto.Rooms?.forEach(room => {
          // Tilføj eller opdater enheden i record
          allRoomRecord![room.Id] = room;
        tempListOfRoomIds.push(room.Id)
        });

      this.allRoomsSubject.next(allRoomRecord);
    });
  }

  ServerDeletesRoom(dto: ServerDeletesRoom) {
    console.log(dto.DeletedRoom)
    if (dto.DeletedRoom) {
      const rooms = { ...this.allRoomsSubject.value };
      delete rooms[dto.DeletedRoom];
      this.allRoomsSubject.next(rooms);
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
}
