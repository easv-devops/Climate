import {BaseDto} from "./baseDto";
import {LatestData} from "./Entities";

export class ServerSendsLatestDeviceReadingsDto extends BaseDto<ServerSendsLatestDeviceReadingsDto> {
  Data!: LatestData;
}
