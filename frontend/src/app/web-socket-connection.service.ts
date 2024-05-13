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
import {Device, DeviceInRoom, Room, SensorDto} from "../models/Entities";
import {ServerSendsDevicesByRoomIdDto} from "../models/ServerSendsDevicesByRoomIdDto";
import {ServerEditsDeviceDto} from "../models/ServerEditsDeviceDto";
import {ServerSendsDevicesByUserIdDto} from "../models/ServerSendsDevicesByUserIdDto";
import {ClientWantsToGetDevicesByUserIdDto} from "../models/ClientWantsToGetDevicesByUserIdDto";
import {ServerSendsTemperatureReadingsDto} from "../models/ServerSendsTemperatureReadingsDto";
import {ServerSendsHumidityReadingsDto} from "../models/ServerSendsHumidityReadingsDto";
import {ServerSendsPm25ReadingsDto} from "../models/ServerSendsPm25ReadingsDto";
import {ServerSendsPm100ReadingsDto} from "../models/ServerSendsPm100ReadingsDto";
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
  }

  ServerSendsErrorMessageToClient(dto: ServerSendsErrorMessageToClient) {
    const errorMessage = 'Something went wrong. ' + dto.errorMessage;
    this.errorHandlingService.handleError(errorMessage);
  }

  ServerRegisterUser(dto: ServerRegisterUserDto) {
    localStorage.setItem("jwt", dto.Jwt!);
    this.jwtSubject.next(dto.Jwt);
  }

  ServerResetsPassword(dto: ServerResetsPasswordDto) {
    this.isResetSubject.next(dto.IsReset);
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


  //todo skal slette deviceId i allRooms record device-liste
  ServerSendsDeviceDeletionStatus(dto: ServerSendsDeviceDeletionStatusDto) {
    if (dto.IsDeleted && this.allDevicesSubject.value) {
      const devices = {...this.allDevicesSubject.value};
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
    this.isDeviceEditedSubject.next(undefined); // Nulstil isDeviceEdited-subjektet
  }

  ServerSendsTemperatureReadings(dto: ServerSendsTemperatureReadingsDto) {
    this.temperatureReadings.pipe(take(1)).subscribe(temperatureReadingsRecord => {
      if (!temperatureReadingsRecord) {
        temperatureReadingsRecord = {};
      }
      // Hent de eksisterende læsninger for det givne DeviceId
      let existingReadings = temperatureReadingsRecord[dto.DeviceId] || [];

      // Tilføj de nye læsninger til de eksisterende læsninger
      existingReadings = existingReadings.concat(dto.TemperatureReadings);

      // Sortér læsningerne efter tidspunkt
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());

      // Opdater temperatureReadingsRecord med de opdaterede læsninger for det specifikke DeviceId
      temperatureReadingsRecord[dto.DeviceId] = existingReadings;

      if (temperatureReadingsRecord){
        // Opdater temperatureReadingsSubject med den opdaterede temperatureReadingsRecord
        this.temperatureReadingsSubject.next(temperatureReadingsRecord);
      }
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
