import {BaseDto} from "./baseDto";
import {LatestData} from "./Entities";

export class ServerSendsLatestRoomReadingsDto extends BaseDto<ServerSendsLatestRoomReadingsDto> {
  Data!: LatestData;
}
