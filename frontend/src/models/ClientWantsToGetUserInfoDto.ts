import {BaseDto} from "./baseDto";

export class ClientWantsToGetUserInfoDto extends BaseDto<ClientWantsToGetUserInfoDto> {
  // Intentionally empty.
  // We need a Dto, but we get the only needed attribute, userId, from the socket connection in backend.
}
