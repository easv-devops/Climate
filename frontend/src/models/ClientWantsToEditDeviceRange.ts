import {BaseDto} from "./baseDto";
import {ClientWantsToEditDeviceDto} from "./ClientWantsToEditDeviceDto";
import {DeviceRange} from "./Entities";

export class ClientWantsToEditDeviceRangeDto extends BaseDto<ClientWantsToEditDeviceRangeDto>{
 Settings!: DeviceRange
}
