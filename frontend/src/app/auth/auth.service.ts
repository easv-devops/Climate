import {Inject} from "@angular/core";
import {WebSocketConnectionService} from "../web-socket-connection.service";

export class AuthService{
  ws = Inject(WebSocketConnectionService);


  loginUser(){
    var object ={
      eventType: "ClientWantsToAuthenticate",
      Email: "Anelise@gmail.com",
      Password: "qwertyuiop"
    }
    this.ws.socketConnection.send(JSON.stringify(object));
  }
}
