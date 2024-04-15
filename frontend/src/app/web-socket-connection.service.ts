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


@Injectable({providedIn: 'root'})
export class WebSocketConnectionService {

  AllRooms: number[] = [];
  AllDevices: number[] = [];
  currentUser: User = {};
  jwt: string | undefined;


  //Socket connection
  public socketConnection: WebSocket;

  constructor() {
    this.socketConnection = new WebSocket("ws://0.0.0.0:8181")
    this.handleEvent();
  }

  handleEvent() {
    this.socketConnection.onmessage = (event) => {
      const data = JSON.parse(event.data) as BaseDto<any>;
      console.log("Recieved: " + JSON.stringify(data));
      // @ts-ignore
      this[data.eventType].call(this,data);
    }
  }

  //All the return objects
  ServerAuthenticatesUser(dto: ServerAuthenticatesUserDto) {
    console.log("Jwt: " + dto.Jwt)
    this.jwt = dto.Jwt;
  }
}
