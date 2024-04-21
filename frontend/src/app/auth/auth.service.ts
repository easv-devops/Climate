import {inject, Inject, Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../web-socket-connection.service";
import {
  ClientWantsToAuthenticate,
  ClientWantsToRegisterDto,
  ClientWantsToResetPassword
} from "../../models/clientRequests";

@Injectable({
  providedIn: 'root'
})
export class AuthService{
  private ws: WebSocketConnectionService = inject(WebSocketConnectionService);


  constructor() {
  }


  registerUser(registerDto: ClientWantsToRegisterDto){
    this.ws.socketConnection.sendDto(registerDto)
  }

  //todo
  loginUser(email: string, password: string){
    this.ws.socketConnection.sendDto(
      new ClientWantsToAuthenticate({
        Email: email,
        Password: password
      })
    )
  }


  resetPasswordWithEmail(clientWantsToResetPassword: ClientWantsToResetPassword){
    this.ws.socketConnection.sendDto(clientWantsToResetPassword)
  }


}


