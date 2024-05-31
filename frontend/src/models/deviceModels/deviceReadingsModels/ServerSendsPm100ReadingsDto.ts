import {BaseDto} from "../../baseDto";
import {SensorDto} from "../../Entities";

export class ServerSendsPm100ReadingsDto extends BaseDto<ServerSendsPm100ReadingsDto> {
  DeviceId!: number;
  Readings!: SensorDto[];
}
