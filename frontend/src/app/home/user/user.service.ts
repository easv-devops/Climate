import {Injectable} from "@angular/core";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToEditUserInfoDto} from "../../../models/userModels/ClientWantsToEditUserInfoDto";

@Injectable({providedIn: 'root'})
export class UserService {

  constructor(private ws: WebSocketConnectionService) {
  }

  editUser(dto: ClientWantsToEditUserInfoDto) {
    this.ws.socketConnection.sendDto(dto)
  }
}
