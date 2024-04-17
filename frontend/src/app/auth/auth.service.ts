import {inject, Inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../web-socket-connection.service";
@Injectable({
  providedIn: 'root'
})
export class AuthService{
  private ws: WebSocketConnectionService = inject(WebSocketConnectionService);


  constructor() {
  }


  loginUser(){
    var object ={
      eventType: "ClientWantsToAuthenticate",
      Email: "user@example.com",
      Password: "12345678"
    }
    this.ws.socketConnection.send(JSON.stringify(object));
  }
}
