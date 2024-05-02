import {BaseDto} from "./baseDto";
import {SensorDto} from "./Entities";

export class ServerSendsHumidityReadingsDto extends BaseDto<ServerSendsHumidityReadingsDto> {
  HumidityReadings!: SensorDto[];
}
