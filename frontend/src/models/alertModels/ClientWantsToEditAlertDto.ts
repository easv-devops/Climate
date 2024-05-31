import {BaseDto} from "../baseDto";

export class ClientWantsToEditAlertDto extends BaseDto<ClientWantsToEditAlertDto> {
  AlertId!: number;
  DeviceId!: number;
  IsRead!: boolean;
}
