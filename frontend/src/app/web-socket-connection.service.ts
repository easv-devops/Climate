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
import {
  AlertDto,
  CountryCode,
  Device,
  DeviceRange,
  DeviceRangeDto, DeviceSettingDto,
  DeviceSettings, LatestData,
  Room,
  SensorDto
} from "../models/Entities";
import {ServerEditsDeviceDto} from "../models/deviceModels/ServerEditsDeviceDto";
import {ServerSendsDevicesByUserIdDto} from "../models/deviceModels/ServerSendsDevicesByUserIdDto";
import {ServerSendsTemperatureReadingsDto} from "../models/deviceModels/deviceReadingsModels/ServerSendsTemperatureReadingsDto";
import {ServerSendsHumidityReadingsDto} from "../models/deviceModels/deviceReadingsModels/ServerSendsHumidityReadingsDto";
import {ServerSendsPm25ReadingsDto} from "../models/deviceModels/deviceReadingsModels/ServerSendsPm25ReadingsDto";
import {ServerSendsPm100ReadingsDto} from "../models/deviceModels/deviceReadingsModels/ServerSendsPm100ReadingsDto";
import {ServerReturnsAllRoomsDto} from "../models/roomModels/ServerReturnsAllRoomsDto";
import {ServerSendsDeviceIdListForRoomDto} from "../models/deviceModels/ServerSendsDeviceIdListForRoomDto";
import {ServerSendsRoom} from "../models/roomModels/ServerSendsRoom";
import {ServerDeletesRoom} from "../models/roomModels/ServerDeletesRoom";
import {FullUserDto, ServerSendsUser} from "../models/userModels/ServerSendsUser";
import {ServerSendsCountryCodesDto} from "../models/ServerSendsCountryCodes";
import {ServerSendsPm25ReadingsForRoom} from "../models/roomModels/roomReadingModels/ServerSendsPm25ReadingsForRoom";
import {ServerSendsTemperatureReadingsForRoom} from "../models/roomModels/roomReadingModels/ServerSendsTemperatureReadingsForRoom";
import {ServerSendsPm100ReadingsForRoom} from "../models/roomModels/roomReadingModels/ServerSendsPm100ReadingsForRoom";
import {ServerSendsHumidityReadingsForRoom} from "../models/roomModels/roomReadingModels/ServerSendsHumidityReadingsForRoom";
import {ServerSendsLatestDeviceReadingsDto} from "../models/deviceModels/deviceReadingsModels/ServerSendsLatestDeviceReadingsDto";
import {ServerSendsLatestRoomReadingsDto} from "../models/roomModels/ServerSendsLatestRoomReadingsDto";
import {ServerSendsAlertList} from "../models/alertModels/ServerSendsAlertList";
import {ServerSendsAlert} from "../models/alertModels/ServerSendsAlert";



@Injectable({providedIn: 'root'})
export class WebSocketConnectionService {

  //Different objects used in the application

  //observable jwt  --remember to unsub when done using (se login JWT ngOnit for more info)
  public jwtSubject = new BehaviorSubject<string | undefined>(undefined);
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


  private allDeviceRangeSettingsSubject = new BehaviorSubject<Record<number, DeviceRange> | undefined>(undefined);
  allDeviceRangeSettings: Observable<Record<number, DeviceRange> | undefined> = this.allDeviceRangeSettingsSubject.asObservable();


  private allDeviceSettingsSubject = new BehaviorSubject<Record<number, DeviceSettings> | undefined>(undefined);
  allDeviceSettings: Observable<Record<number, DeviceSettings> | undefined> = this.allDeviceSettingsSubject.asObservable();

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

  private latestDeviceReadingsSubject = new BehaviorSubject<Record<number, LatestData> | undefined>(undefined);
  latestDeviceReadings: Observable<Record<number, LatestData> | undefined> = this.latestDeviceReadingsSubject.asObservable();

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

  private latestRoomReadingsSubject = new BehaviorSubject<Record<number, LatestData> | undefined>(undefined);
  latestRoomReadings: Observable<Record<number, LatestData> | undefined> = this.latestRoomReadingsSubject.asObservable();

  // Observable for alerts
  private alertsSubject = new BehaviorSubject<AlertDto[] | undefined>(undefined);
  alerts: Observable<AlertDto[] | undefined> = this.alertsSubject.asObservable();


  constructor(private errorHandlingService: ErrorHandlingService) {
    //Pointing to the direction the websocket can be found at
    this.socketConnection = new WebsocketSuperclass(environment.websocketBaseUrl, this);
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


  ServerSendsDeviceRangeSettings(dto: DeviceRangeDto) {
    this.allDeviceRangeSettings.pipe(take(1)).subscribe(allDevicesRecord => {
      if (!allDevicesRecord) {
        allDevicesRecord = {};
      }
      // Add or update the device range settings in the record
      allDevicesRecord[dto.Settings.Id] = dto.Settings;

      // Update allDeviceSettingsSubject with the updated record
      this.allDeviceRangeSettingsSubject.next(allDevicesRecord);
    });
  }

  ServerSendsDeviceSettings(dto: DeviceSettingDto) {
    this.allDeviceSettings.pipe(take(1)).subscribe(allDevicesRecord => {
      if (!allDevicesRecord) {
        allDevicesRecord = {};
      }

      // Add or update the device range settings in the record
      allDevicesRecord[dto.Settings.Id] = dto.Settings;

      // Update allDeviceSettingsSubject with the updated record
      this.allDeviceSettingsSubject.next(allDevicesRecord);
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
    this.jwtSubject.next(undefined);
    this.isResetSubject.next(undefined);
    this.isDeletedSubject.next(undefined);
    this.deviceIdSubject.next(undefined);
    this.userSubject.next(undefined);
    this.allCountryCodesSubject.next(undefined);
    this.allDevicesSubject.next(undefined);
    this.allRoomsSubject.next(undefined);
    this.isDeviceEditedSubject.next(undefined);
    this.allDeviceRangeSettingsSubject.next(undefined);
    this.allDeviceSettingsSubject.next(undefined);
    this.temperatureReadingsSubject.next(undefined);
    this.humidityReadingsSubject.next(undefined);
    this.pm25ReadingsSubject.next(undefined);
    this.pm100ReadingsSubject.next(undefined);
    this.latestDeviceReadingsSubject.next(undefined);
    this.temperatureRoomReadingsSubject.next(undefined);
    this.humidityRoomReadingsSubject.next(undefined);
    this.pm25RoomReadingsSubject.next(undefined);
    this.pm100RoomReadingsSubject.next(undefined);
    this.latestRoomReadingsSubject.next(undefined);
    this.alertsSubject.next(undefined);
  }

  ServerSendsTemperatureReadings(dto: ServerSendsTemperatureReadingsDto) {
    this.temperatureReadings.pipe(take(1)).subscribe(temperatureReadingsRecord => {
      // Initialiser temperatureReadingsRecord til et tomt objekt, hvis det er null eller udefineret
      temperatureReadingsRecord = temperatureReadingsRecord || {};

      // Hent de eksisterende læsninger for det givne DeviceId
      let existingReadings = temperatureReadingsRecord[dto.DeviceId] || [];

      // Tilføj de nye læsninger til de eksisterende læsninger
      existingReadings = existingReadings.concat(dto.Readings);

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
      existingReadings = existingReadings.concat(dto.Readings);

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
      existingReadings = existingReadings.concat(dto.Readings);

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
      existingReadings = existingReadings.concat(dto.Readings);
      // Sort the readings by timestamp
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());
      // Update pm100ReadingsRecord with the updated readings for the specific DeviceId
      pm100ReadingsRecord[dto.DeviceId] = existingReadings;
      // Update pm100ReadingsSubject with the updated pm100ReadingsRecord
      this.pm100ReadingsSubject.next(pm100ReadingsRecord);
    });
  }

  ServerSendsLatestDeviceReadings(dto: ServerSendsLatestDeviceReadingsDto) {
    this.latestDeviceReadings.pipe(take(1)).subscribe(latestDeviceRecord => {
      // Initialize the record if it is undefined
      const updatedRecord: Record<number, LatestData> = latestDeviceRecord ? { ...latestDeviceRecord } : {};

      // Create or update the entry for the device
      updatedRecord[dto.Data.Id] = {
        Id: dto.Data.Id,
        Data: dto.Data.Data,
      };

      // Emit the updated record
      this.latestDeviceReadingsSubject.next(updatedRecord);
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

      existingReadings = this.removeDuplicateTimestamp(existingReadings, dto.Readings);

      existingReadings = existingReadings.concat(dto.Readings);

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

      existingReadings = this.removeDuplicateTimestamp(existingReadings, dto.Readings);

      // Add the new readings to the existing readings
      existingReadings = existingReadings.concat(dto.Readings);

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

      existingReadings = this.removeDuplicateTimestamp(existingReadings, dto.Readings);

      // Add the new readings to the existing readings
      existingReadings = existingReadings.concat(dto.Readings);

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

      existingReadings = this.removeDuplicateTimestamp(existingReadings, dto.Readings);

      // Add the new readings to the existing readings
      existingReadings = existingReadings.concat(dto.Readings);
      // Sort the readings by timestamp
      existingReadings.sort((a, b) => new Date(a.TimeStamp).getTime() - new Date(b.TimeStamp).getTime());
      // Update pm100ReadingsRecord with the updated readings for the specific DeviceId
      pm100ReadingsRecord[dto.RoomId] = existingReadings;
      // Update pm100ReadingsSubject with the updated pm100ReadingsRecord
      this.pm100RoomReadingsSubject.next(pm100ReadingsRecord);
    });
  }


  /**
   * Since we are averaging readings in a 120 minute interval, it's likely to produce a new SensorDto with
   * a duplicated Timestamp that now has a new Value. This method checks for old values and removes them.
   */
  private removeDuplicateTimestamp(existingReadings: SensorDto[], newReadings: SensorDto[]): SensorDto[] {
    // Check if there are existing readings
    if (existingReadings.length > 0) {
      // Check if the last reading in the existing readings has the same timestamp as the last reading in the new readings
      const lastExistingReading = existingReadings[existingReadings.length - 1];
      const lastNewReading = newReadings[newReadings.length - 1];
      if (lastExistingReading && lastNewReading && lastExistingReading.TimeStamp === lastNewReading.TimeStamp) {
        // If the timestamps match, remove the last reading from existing readings
        existingReadings = existingReadings.slice(0, -1);
      }
    }

    return existingReadings;
  }

  ServerSendsLatestRoomReadings(dto: ServerSendsLatestRoomReadingsDto) {
    this.latestRoomReadings.pipe(take(1)).subscribe(latestRoomRecord => {
      // Initialize the record if it is undefined
      const updatedRecord: Record<number, LatestData> = latestRoomRecord ? { ...latestRoomRecord } : {};

      // Create or update the entry for the room
      updatedRecord[dto.Data.Id] = {
        Id: dto.Data.Id,
        Data: dto.Data.Data,
      };

      // Emit the updated record
      this.latestRoomReadingsSubject.next(updatedRecord);
    });
  }

  ServerSendsAlertList(dto: ServerSendsAlertList) {
    this.alerts.pipe(take(1)).subscribe(alertList => {
      if (!alertList) {
        alertList = [];
      }

      dto.Alerts?.forEach(alert => {
        alertList?.push(alert)
      });

      this.alertsSubject.next(alertList);
    });
  }

  ServerSendsAlert(dto: ServerSendsAlert) {
    this.alerts.pipe(take(1)).subscribe(alertList => {
      if(alertList) {
        const updatedAlertList = alertList.map(alert => {
          return alert.Id === dto.Alert.Id ? dto.Alert : alert;
        });
        this.alertsSubject.next(updatedAlertList);
      }
    })
  }
}
