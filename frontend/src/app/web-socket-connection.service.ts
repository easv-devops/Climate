import {Injectable} from "@angular/core";

class BaseDto<T> {
}
class ResponseDto<T> {
  responseData?: T;
  messageToClient?: string;
}

class User {
  firstname?: string;
  lastname?: string;
  email?: string;
}

class ServerAuthenticatesUserDto {
  Jwt?: string;
}

class ShortUserDto {
  Id?: number;
  Email?: string;
}
@Injectable({providedIn: 'root'})
export class WebSocketConnectionService {

  AllRooms: number[] = [];
  AllDevices: number[] = [];
  currentUser: User = {};
  jwt: string | undefined;


  //Socket connection
  public socketConnection: WebSocket;// = new WebSocket("ws://localhost:8181")

  constructor() {
    //pointing to the direction the websocket can be found at
    this.socketConnection = new WebSocket("ws://localhost:8181")
    this.handleEvent();
  }

  handleEvent() {
    this.socketConnection.onmessage = (event) => {
      const data = JSON.parse(event.data) as BaseDto<any>;
      // @ts-ignore
      this[data.eventType].call(this,data);
    }

  }

  //All the return objects

  ServerAuthenticatesUser(dto: ServerAuthenticatesUserDto) {
    this.jwt = dto.Jwt;
    console.log("Recieved token: " + this.jwt);
  }
  /*

  ServerAuthenticatesUser(dto: ShortUserDto){
    console.log("Server authenticated user")
  }*/
}
