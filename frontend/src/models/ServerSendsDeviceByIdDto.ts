import { Device } from "./Entities";
import {BaseDto} from "./baseDto";

export class ServerSendsDeviceByIdDto extends BaseDto<ServerSendsDeviceByIdDto>{
  Device?: Device
}
