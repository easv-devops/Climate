import {BaseDto} from "../../baseDto";
import {SensorDto} from "../../Entities";

export class ServerSendsTemperatureReadingsForRoom extends BaseDto<ServerSendsTemperatureReadingsForRoom> {
  RoomId!: number;
  TemperatureReadings!: SensorDto[];
}
