import {BaseDto} from "../../baseDto";

export class ClientWantsToGetHumidityReadingsForRoomDto extends BaseDto<ClientWantsToGetHumidityReadingsForRoomDto>{
  RoomId?: number;
  StartTime?: Date;
  EndTime?: Date;
  Interval?: number;
}
