import {BaseDto} from "../../baseDto";
import {SensorDto} from "../../Entities";

export class ServerSendsHumidityReadingsForRoom extends BaseDto<ServerSendsHumidityReadingsForRoom> {
  RoomId!: number;
  HumidityReadings!: SensorDto[];
}
