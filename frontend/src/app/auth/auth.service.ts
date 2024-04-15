import {Inject} from "@angular/core";
import {WebSocketConnectionService} from "../web-socket-connection.service";

export class AuthService{
  ws = Inject(WebSocketConnectionService);


  loginUser(){
    this.ws.socketConnection.sendDto()
  }
}
