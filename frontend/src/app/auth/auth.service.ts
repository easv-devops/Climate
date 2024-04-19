import {inject, Inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../web-socket-connection.service";
import {BaseDto} from "../../models/baseDto";
import {ClientWantsToAuthenticate, ClientWantsToRegister} from "../../models/clientRequests";
@Injectable({
  providedIn: 'root'
})
export class AuthService{
  private ws: WebSocketConnectionService = inject(WebSocketConnectionService);


  constructor() {
  }


  //TODO fix the register
  registerUser(){
    this.ws.socketConnection.sendDto(
      new ClientWantsToRegister({
        Email: "",
        Phone: "",
        Name: "",
        Password: ""
      })
    )
  }
  loginUser(email: string, password: string){
    this.ws.socketConnection.sendDto(
      new ClientWantsToAuthenticate({
        Email: email,
        Password: password
      })
    )
  }
}
