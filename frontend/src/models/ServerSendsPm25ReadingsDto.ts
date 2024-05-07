import {BaseDto} from "./baseDto";
import {SensorDto} from "./Entities";

export class ServerSendsPm25ReadingsDto extends BaseDto<ServerSendsPm25ReadingsDto> {
  DeviceId!: number;
  Pm25Readings!: SensorDto[];
}
