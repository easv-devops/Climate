import {Injectable} from "@angular/core";
import {environment} from "../environments/environment";
import {WebsocketSuperclass} from "../models/websocketSuperclass";
import {BaseDto} from "../models/baseDto";
import {
  ServerAuthenticatesUserDto,
  ServerRegisterUserDto
  , ServerResetsPasswordDto, ServerReturnsAllRoomsDto, ServerSendsErrorMessageToClient,
} from "../models/returnedObjectsFromBackend";
import {BehaviorSubject, Observable} from "rxjs";
import {ErrorHandlingService} from "./error-handling.service";
import {Device} from "../models/device";
import {Room} from "../models/room";
import {RecordHolder} from "../models/recordholder";

@Injectable({providedIn: 'root'})
export class WebSocketConnectionService {

  //Different objects used in the application
  //todo maybe not "AllDevices" but just "devices". We should lazy load with longer json elements.
  allRoomsRecord!: RecordHolder<Room>;
  AllDevicesRecord!: RecordHolder<Device>


  //todo we should maybe have an endpoint for getting a user we can call when hitting the main page

  //observable jwt  --remember to unsub when done using (se login JWT ngOnit for more info)
  private jwtSubject = new BehaviorSubject<string | undefined>(undefined);
  jwt: Observable<string | undefined> = this.jwtSubject.asObservable();

  //used to reset password
  private isResetSubject = new BehaviorSubject<boolean | undefined>(undefined);
  isReset: Observable<boolean | undefined> = this.isResetSubject.asObservable();

  //Socket connection
  public socketConnection: WebsocketSuperclass;

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
  ServerReturnsAllRooms(dto: ServerReturnsAllRoomsDto){
    for (var room of dto.rooms!) {
      this.allRoomsRecord!.addRecord(room.roomId, room)
    }
  }
}

