import {BaseDto} from "./baseDto";
import {DeviceSettings} from "./Entities";

export class ClientWantsToEditDeviceSettingsDto extends BaseDto<ClientWantsToEditDeviceSettingsDto>{
  Settings!: DeviceSettings
}
