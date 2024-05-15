import {BaseDto} from "../../baseDto";

export class ClientWantsToGetPm100ReadingsForRoomDto extends BaseDto<ClientWantsToGetPm100ReadingsForRoomDto>{
  RoomId?: number;
  StartTime?: Date;
  EndTime?: Date;
  Interval?: number;
}
