import {BaseDto} from "./baseDto";
import {SensorDto} from "./Entities";

export class ServerSendsTemperatureReadingsDto extends BaseDto<ServerSendsTemperatureReadingsDto> {
  TemperatureReadings!: SensorDto[];
}
