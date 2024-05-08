import {BaseDto} from "../baseDto";
import {ClientWantsToGetDevicesByUserIdDto} from "../ClientWantsToGetDevicesByUserIdDto";


export class ClientWantsToGetAllRoomsDto extends BaseDto<ClientWantsToGetAllRoomsDto> {
  // Intentionally empty.
  // We need a Dto, but we get the only needed attribute, userId, from the socket connection in backend.
}
