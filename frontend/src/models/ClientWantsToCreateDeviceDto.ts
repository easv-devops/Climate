import {BaseDto} from "./baseDto";

export class ClientWantsToCreateDeviceDto extends BaseDto<ClientWantsToCreateDeviceDto>{
  DeviceName?: string;
  RoomId?: number;
}
