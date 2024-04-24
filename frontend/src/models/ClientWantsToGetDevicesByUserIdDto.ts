import {BaseDto} from "./baseDto";

export class ClientWantsToGetDevicesByUserIdDto extends BaseDto<ClientWantsToGetDevicesByUserIdDto> {
  // Intentionally empty.
  // We need a Dto, but we get the only needed attribute, userId, from the socket connection in backend.
}
