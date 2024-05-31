import {BaseDto} from "../../baseDto";

export class ClientWantsToGetLatestDeviceReadingsDto extends BaseDto<ClientWantsToGetLatestDeviceReadingsDto> {
  DeviceId!: number;
}
