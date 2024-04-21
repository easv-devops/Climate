import {Injectable} from "@angular/core";
import {environment} from "../environments/environment";
import {WebsocketSuperclass} from "../models/websocketSuperclass";
import {BaseDto} from "../models/baseDto";
import {ServerAuthenticatesUserDto, User} from "../models/returnedObjectsFromBackend";

@Injectable({providedIn: 'root'})
export class WebSocketConnectionService {

  //Different objects used in the application
  //TODO check for these objects. Make sure they are used or removed
  //TODO use records instead of lists
  //todo should be objects instead of number, reference to the object (key= id. value = object)
  //todo maybe not "AllDevices" but just "devices". We should lazy load with longer json elements.
  AllRooms: number[] = [];
  AllDevices: number[] = [];
  jwt: string | undefined;

  //Socket connection
  public socketConnection: WebsocketSuperclass;

  constructor() {
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
    this.jwt = dto.Jwt;
  }
}

