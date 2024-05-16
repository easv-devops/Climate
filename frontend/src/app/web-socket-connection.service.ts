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
import {CountryCode, Device, Room, SensorDto} from "../models/Entities";
import {ServerEditsDeviceDto} from "../models/ServerEditsDeviceDto";
import {ServerSendsDevicesByUserIdDto} from "../models/ServerSendsDevicesByUserIdDto";
import {ServerSendsTemperatureReadingsDto} from "../models/ServerSendsTemperatureReadingsDto";
import {ServerSendsHumidityReadingsDto} from "../models/ServerSendsHumidityReadingsDto";
import {ServerSendsPm25ReadingsDto} from "../models/ServerSendsPm25ReadingsDto";
import {ServerSendsPm100ReadingsDto} from "../models/ServerSendsPm100ReadingsDto";
import {ServerReturnsAllRoomsDto} from "../models/roomModels/ServerReturnsAllRoomsDto";
import {ServerSendsDeviceIdListForRoomDto} from "../models/ServerSendsDeviceIdListForRoomDto";
import {ServerSendsRoom} from "../models/roomModels/ServerSendsRoom";
import {ServerDeletesRoom} from "../models/roomModels/ServerDeletesRoom";
import {FullUserDto, ServerSendsUser} from "../models/ServerSendsUser";
import {ServerSendsCountryCodesDto} from "../models/ServerSendsCountryCodes";
import {ServerSendsPm25ReadingsForRoom} from "../models/roomModels/roomReadingModels/ServerSendsPm25ReadingsForRoom";
import {ServerSendsTemperatureReadingsForRoom} from "../models/roomModels/roomReadingModels/ServerSendsTemperatureReadingsForRoom";
import {ServerSendsPm100ReadingsForRoom} from "../models/roomModels/roomReadingModels/ServerSendsPm100ReadingsForRoom";
import {ServerSendsHumidityReadingsForRoom} from "../models/roomModels/roomReadingModels/ServerSendsHumidityReadingsForRoom";


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

  private userSubject = new BehaviorSubject<FullUserDto | undefined>(undefined);
  user: Observable<FullUserDto | undefined> = this.userSubject.asObservable();

  private allCountryCodesSubject = new BehaviorSubject<CountryCode[] | undefined>(undefined);
  allCountryCodes: Observable<CountryCode[] | undefined> = this.allCountryCodesSubject.asObservable();

  private allDevicesSubject = new BehaviorSubject<Record<number, Device> | undefined>(undefined);
  allDevices: Observable<Record<number, Device> | undefined> = this.allDevicesSubject.asObservable();

  private allRoomsSubject = new BehaviorSubject<Record<number, Room> | undefined>(undefined);
  allRooms: Observable<Record<number, Room> | undefined> = this.allRoomsSubject.asObservable();

  private isDeviceEditedSubject = new BehaviorSubject<boolean | undefined>(undefined);
  isDeviceEdited: Observable<boolean | undefined> = this.isDeviceEditedSubject.asObservable();


  /**
   * observables for device readings
   * @private
   */
  private temperatureReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  temperatureReadings: Observable<Record<number, SensorDto[]> | undefined> = this.temperatureReadingsSubject.asObservable();

  private humidityReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  humidityReadings: Observable<Record<number, SensorDto[]> | undefined> = this.humidityReadingsSubject.asObservable();

  private pm25ReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  pm25Readings: Observable<Record<number, SensorDto[]> | undefined> = this.pm25ReadingsSubject.asObservable();

  private pm100ReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  pm100Readings: Observable<Record<number, SensorDto[]> | undefined> = this.pm100ReadingsSubject.asObservable();


  /**
   * observables for room readings
   * @private
   */
  private temperatureRoomReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  temperatureRoomReadings: Observable<Record<number, SensorDto[]> | undefined> = this.temperatureRoomReadingsSubject.asObservable();

  private humidityRoomReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  humidityRoomReadings: Observable<Record<number, SensorDto[]> | undefined> = this.humidityRoomReadingsSubject.asObservable();

  private pm25RoomReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  pm25RoomReadings: Observable<Record<number, SensorDto[]> | undefined> = this.pm25RoomReadingsSubject.asObservable();

  private pm100RoomReadingsSubject = new BehaviorSubject<Record<number, SensorDto[]> | undefined>(undefined);
  pm100RoomReadings: Observable<Record<number, SensorDto[]> | undefined> = this.pm100RoomReadingsSubject.asObservable();

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

  ServerSendsCountryCodes(dto: ServerSendsCountryCodesDto) {
    this.allCountryCodesSubject.next(dto.CountryCode)
  }

  ServerRegisterUser(dto: ServerRegisterUserDto) {
    localStorage.setItem("jwt", dto.Jwt!);
    this.jwtSubject.next(dto.Jwt);
  }

  ServerResetsPassword(dto: ServerResetsPasswordDto) {
    this.isResetSubject.next(dto.IsReset);
  }

  ServerSendsUser(dto: ServerSendsUser) {
    this.user.pipe(take(1)).subscribe(user => {
      user = dto.UserDto;
      this.userSubject.next(user);
    });
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
        const updatedRoom = {...roomsSnapshot[dto.RoomId]};

        updatedRoom.DeviceIds = dto.DeviceIds;
        const updatedRoomsSnapshot = {...roomsSnapshot, [dto.RoomId]: updatedRoom};
        // Udsend den opdaterede snapshot
        this.allRoomsSubject.next(updatedRoomsSnapshot);
      }
    });
  }

  ServerReturnsAllRooms(dto: ServerReturnsAllRoomsDto) {
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
      const rooms = {...this.allRoomsSubject.value};
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
    this.userSubject.next(undefined); // Nulstil allUsers-subjektet
    this.allCountryCodesSubject.next(undefined); // Nulstil allCountryCodes-subjektet
  }

  ServerSendsTemperatureReadings(dto: ServerSendsTemperatureReadingsDto) {
    this.temperatureReadings.pipe(take(1)).subscribe(temperatureReadingsRecord => {
      // Initialiser temperatureReadingsRecord til et tomt objekt, hvis det er null eller udefineret
      temperatureReadingsRecord = temperatureReadingsRecord || {};

      // Hent de eksisterende læsninger for det givne DeviceId
      let existingReadings = temperatureReadingsRecord[dto.DeviceId] || [];

      // Tilføj de nye læsninger til de eksisterende læsninger
      existingReadings = existingReadings.concat(dto.TemperatureReadings);

      // Sortér læsningerne efter tidspunkt
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());

      // Opdater temperatureReadingsRecord med de opdaterede læsninger for det specifikke DeviceId
      temperatureReadingsRecord[dto.DeviceId] = existingReadings;

      // Opdater temperatureReadingsSubject med den opdaterede temperatureReadingsRecord
      this.temperatureReadingsSubject.next(temperatureReadingsRecord);
    });
  }

  ServerSendsHumidityReadings(dto: ServerSendsHumidityReadingsDto) {
    this.humidityReadings.pipe(take(1)).subscribe(humidityReadingsRecord => {
      // Initialize humidityReadingsRecord to an empty object if it's null or undefined
      humidityReadingsRecord = humidityReadingsRecord || {};

      // Get the existing readings for the given DeviceId
      let existingReadings = humidityReadingsRecord[dto.DeviceId] || [];

      // Add the new readings to the existing readings
      existingReadings = existingReadings.concat(dto.HumidityReadings);

      // Sort the readings by timestamp
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());

      // Update humidityReadingsRecord with the updated readings for the specific DeviceId
      humidityReadingsRecord[dto.DeviceId] = existingReadings;

      // Update humidityReadingsSubject with the updated humidityReadingsRecord
      this.humidityReadingsSubject.next(humidityReadingsRecord);
    });
  }

  ServerSendsPm25Readings(dto: ServerSendsPm25ReadingsDto) {
    this.pm25Readings.pipe(take(1)).subscribe(pm25ReadingsRecord => {
      // Initialize pm25ReadingsRecord to an empty object if it's null or undefined
      pm25ReadingsRecord = pm25ReadingsRecord || {};

      // Get the existing readings for the given DeviceId
      let existingReadings = pm25ReadingsRecord[dto.DeviceId] || [];

      // Add the new readings to the existing readings
      existingReadings = existingReadings.concat(dto.Pm25Readings);

      // Sort the readings by timestamp
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());

      // Update pm25ReadingsRecord with the updated readings for the specific DeviceId
      pm25ReadingsRecord[dto.DeviceId] = existingReadings;

      // Update pm25ReadingsSubject with the updated pm25ReadingsRecord
      this.pm25ReadingsSubject.next(pm25ReadingsRecord);
    });
  }

  ServerSendsPm100Readings(dto: ServerSendsPm100ReadingsDto) {
    this.pm100Readings.pipe(take(1)).subscribe(pm100ReadingsRecord => {
      // Initialize pm100ReadingsRecord to an empty object if it's null or undefined
      pm100ReadingsRecord = pm100ReadingsRecord || {};
      // Get the existing readings for the given DeviceId
      let existingReadings = pm100ReadingsRecord[dto.DeviceId] || [];
      // Add the new readings to the existing readings
      existingReadings = existingReadings.concat(dto.Pm100Readings);
      // Sort the readings by timestamp
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());
      // Update pm100ReadingsRecord with the updated readings for the specific DeviceId
      pm100ReadingsRecord[dto.DeviceId] = existingReadings;
      // Update pm100ReadingsSubject with the updated pm100ReadingsRecord
      this.pm100ReadingsSubject.next(pm100ReadingsRecord);
    });
  }

  /**
   * gets readings from room
   * @param dto
   * @constructor
   */
  ServerSendsTemperatureReadingsForRoom(dto: ServerSendsTemperatureReadingsForRoom) {

    this.temperatureRoomReadings.pipe(take(1)).subscribe(temperatureReadingsRecord => {
      temperatureReadingsRecord = temperatureReadingsRecord || {};
      let existingReadings = temperatureReadingsRecord[dto.RoomId] || [];
      existingReadings = existingReadings.concat(dto.TemperatureReadings);

      // Sortér læsningerne efter tidspunkt
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());

      // Opdater temperatureReadingsRecord med de opdaterede læsninger for det specifikke DeviceId
      temperatureReadingsRecord[dto.RoomId] = existingReadings;

      // Opdater temperatureReadingsSubject med den opdaterede temperatureReadingsRecord
      this.temperatureRoomReadingsSubject.next(temperatureReadingsRecord);
    });
  }

  ServerSendsHumidityReadingsForRoom(dto: ServerSendsHumidityReadingsForRoom) {
    this.humidityRoomReadings.pipe(take(1)).subscribe(humidityReadingsRecord => {
      // Initialize humidityReadingsRecord to an empty object if it's null or undefined
      humidityReadingsRecord = humidityReadingsRecord || {};

      // Get the existing readings for the given DeviceId
      let existingReadings = humidityReadingsRecord[dto.RoomId] || [];

      // Add the new readings to the existing readings
      existingReadings = existingReadings.concat(dto.HumidityReadings);

      // Sort the readings by timestamp
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());

      // Update humidityReadingsRecord with the updated readings for the specific DeviceId
      humidityReadingsRecord[dto.RoomId] = existingReadings;

      // Update humidityReadingsSubject with the updated humidityReadingsRecord
      this.humidityRoomReadingsSubject.next(humidityReadingsRecord);
    });
  }

  ServerSendsPm25ReadingsForRoom(dto: ServerSendsPm25ReadingsForRoom) {
    this.pm25RoomReadings.pipe(take(1)).subscribe(pm25ReadingsRecord => {
      // Initialize pm25ReadingsRecord to an empty object if it's null or undefined
      pm25ReadingsRecord = pm25ReadingsRecord || {};

      // Get the existing readings for the given DeviceId
      let existingReadings = pm25ReadingsRecord[dto.RoomId] || [];

      // Add the new readings to the existing readings
      existingReadings = existingReadings.concat(dto.Pm25Readings);

      // Sort the readings by timestamp
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());

      // Update pm25ReadingsRecord with the updated readings for the specific DeviceId
      pm25ReadingsRecord[dto.RoomId] = existingReadings;

      // Update pm25ReadingsSubject with the updated pm25ReadingsRecord
      this.pm25RoomReadingsSubject.next(pm25ReadingsRecord);
    });
  }

  ServerSendsPm100ReadingsForRoom(dto: ServerSendsPm100ReadingsForRoom) {
    this.pm100RoomReadings.pipe(take(1)).subscribe(pm100ReadingsRecord => {
      // Initialize pm100ReadingsRecord to an empty object if it's null or undefined
      pm100ReadingsRecord = pm100ReadingsRecord || {};
      // Get the existing readings for the given DeviceId
      let existingReadings = pm100ReadingsRecord[dto.RoomId] || [];
      // Add the new readings to the existing readings
      existingReadings = existingReadings.concat(dto.Pm100Readings);
      // Sort the readings by timestamp
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());
      // Update pm100ReadingsRecord with the updated readings for the specific DeviceId
      pm100ReadingsRecord[dto.RoomId] = existingReadings;
      // Update pm100ReadingsSubject with the updated pm100ReadingsRecord
      this.pm100RoomReadingsSubject.next(pm100ReadingsRecord);
    });
  }
}
