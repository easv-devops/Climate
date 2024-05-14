import {BaseDto} from "./baseDto";
import {ClientWantsToAuthenticateWithJwt} from "./clientRequests";
import ReconnectingWebSocket from "reconnecting-websocket";
import {ClientWantsToGetDevicesByUserIdDto} from "./ClientWantsToGetDevicesByUserIdDto";
import {ClientWantsToGetAllRoomsDto} from "./roomModels/clientWantsToGetAllRoomsDto";

/**
 * Using Reconnecting WebSocket because if the user loses connection, they don't have to re-type email and password
 */
export class WebsocketSuperclass extends ReconnectingWebSocket {
  //This array makes sure, all messages are sent one by one
  private messageQueue: Array<BaseDto<any>> = [];

  constructor(address: string) {
    super(address);
    this.onopen = this.handleOpen.bind(this);
  }

  sendDto(dto: BaseDto<any>) {
    //If connection is open: Send the objects, from the que
    //Else: Add the object to the que
    if (this.readyState === WebSocket.OPEN) {
      this.send(JSON.stringify(dto));
    } else {
      this.messageQueue.push(dto);
    }
  }

  private handleOpen() {
    let jwt = localStorage.getItem('jwt');
    localStorage.setItem("jwt", "");
    //If there is a token, we want to login with it
    if (jwt && jwt != '')
      this.sendDto(new ClientWantsToAuthenticateWithJwt({jwt: jwt}));

    this.messageQueue.forEach( dto => {
      if (dto) {
        this.send(JSON.stringify(dto));
      }
    })
    this.messageQueue = []

  }
}
