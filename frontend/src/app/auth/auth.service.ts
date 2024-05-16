import {Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../web-socket-connection.service";
import {
  ClientWantsToAuthenticate,
  ClientWantsToRegisterDto,
  ClientWantsToResetPassword,
} from "../../models/clientRequests";
import {ClientWantsToGetCountryCodeDto} from "../../models/ClientWantsToGetCountryCode";


@Injectable({
  providedIn: 'root'
})
export class AuthService{

  constructor(private ws: WebSocketConnectionService) {
  }

  registerUser(registerDto: ClientWantsToRegisterDto){
    this.ws.socketConnection.sendDto(registerDto)
  }

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

  getCountryCodes(){
    this.ws.socketConnection.sendDto(new ClientWantsToGetCountryCodeDto())
  }
}


