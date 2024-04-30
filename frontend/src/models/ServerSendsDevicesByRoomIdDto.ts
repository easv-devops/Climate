import {BaseDto} from "./baseDto";
import {Device} from "./Entities";

export class ServerSendsDevicesByRoomIdDto extends BaseDto<ServerSendsDevicesByRoomIdDto>{
  Devices?: Device[]
}
