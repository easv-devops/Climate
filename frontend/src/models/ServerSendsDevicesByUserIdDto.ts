import {BaseDto} from "./baseDto";
import {Device} from "./Entities";

export class ServerSendsDevicesByUserIdDto extends BaseDto<ServerSendsDevicesByUserIdDto>{
  Devices?: Device[]
}
