import {BaseDto} from "./baseDto";

export class ClientWantsToEditDeviceDto extends BaseDto<ClientWantsToEditDeviceDto>{
  Id!: number;
  DeviceName?: string;
  RoomId?: number;
}
