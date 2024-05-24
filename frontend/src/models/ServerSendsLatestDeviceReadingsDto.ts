import {BaseDto} from "./baseDto";
import {LatestDeviceData} from "./Entities";

export class ServerSendsLatestDeviceReadingsDto extends BaseDto<ServerSendsLatestDeviceReadingsDto> {
  Data!: LatestDeviceData;
}
