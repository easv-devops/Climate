import {BaseDto} from "../../baseDto";

export class ClientWantsToGetPm25ReadingsForRoomDto extends BaseDto<ClientWantsToGetPm25ReadingsForRoomDto>{
  RoomId?: number;
  StartTime?: Date;
  EndTime?: Date;
  Interval?: number;
}
