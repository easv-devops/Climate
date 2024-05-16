import {SensorDto} from "../../Entities";
import {BaseDto} from "../../baseDto";

export class ServerSendsPm25ReadingsForRoom extends BaseDto<ServerSendsPm25ReadingsForRoom> {
  RoomId!: number;
  Pm25Readings!: SensorDto[];
}
