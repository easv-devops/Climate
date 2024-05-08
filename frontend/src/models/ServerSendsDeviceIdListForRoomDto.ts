import {BaseDto} from "./baseDto";

export class ServerSendsDeviceIdListForRoomDto extends BaseDto<ServerSendsDeviceIdListForRoomDto>{
  DeviceIds!: number[];
  RoomId!: number;
}
