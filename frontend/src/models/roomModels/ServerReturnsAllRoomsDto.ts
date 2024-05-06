import {BaseDto} from "../baseDto";
import {Device, Room} from "../Entities";

export class ServerReturnsAllRoomsDto extends BaseDto<ServerReturnsAllRoomsDto>{
  Rooms!: Room[]
}
