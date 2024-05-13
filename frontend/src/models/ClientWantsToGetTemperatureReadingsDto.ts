import {BaseDto} from "./baseDto";

export class ClientWantsToGetTemperatureReadingsDto extends BaseDto<ClientWantsToGetTemperatureReadingsDto>{
  DeviceId?: number;
  StartTime?: Date;
  EndTime?: Date;
}
