import {BaseDto} from "./baseDto";

export class ClientWantsToGetAlertsDto extends BaseDto<ClientWantsToGetAlertsDto> {
  IsRead!: boolean;
}
