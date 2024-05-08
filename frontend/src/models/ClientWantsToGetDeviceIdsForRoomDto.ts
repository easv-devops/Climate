import {BaseDto} from "./baseDto";

export class ClientWantsToGetDeviceIdsForRoomDto extends BaseDto<ClientWantsToGetDeviceIdsForRoomDto>{
  RoomId!: number;
}
