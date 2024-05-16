import {SensorDto} from "../../Entities";
import {BaseDto} from "../../baseDto";


export class ServerSendsTemperatureReadingsForRoom extends BaseDto<ServerSendsTemperatureReadingsForRoom> {
  RoomId!: number;
  TemperatureReadings!: SensorDto[];
}
