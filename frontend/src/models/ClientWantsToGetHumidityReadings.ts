import {BaseDto} from "./baseDto";

export class ClientWantsToGetHumidityReadingsDto extends BaseDto<ClientWantsToGetHumidityReadingsDto>{
  DeviceId?: number;
  StartTime?: Date;
  EndTime?: Date;
}
