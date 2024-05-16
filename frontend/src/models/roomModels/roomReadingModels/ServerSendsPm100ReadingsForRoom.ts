import {BaseDto} from "../../baseDto";
import {SensorDto} from "../../Entities";

export class ServerSendsPm100ReadingsForRoom extends BaseDto<ServerSendsPm100ReadingsForRoom> {
  RoomId!: number;
  Pm100Readings!: SensorDto[];
}
