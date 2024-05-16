import {BaseDto} from "../../baseDto";

export class ClientWantsToGetTemperatureReadingsForRoomDto extends BaseDto<ClientWantsToGetTemperatureReadingsForRoomDto>{
  RoomId?: number;
  StartTime?: Date;
  EndTime?: Date;
  Interval?: number;
}
