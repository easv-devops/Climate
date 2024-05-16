import {BaseDto} from "./baseDto";
import {FullUserDto} from "./ServerSendsUser";

export class ClientWantsToEditUserInfoDto extends BaseDto<ClientWantsToEditUserInfoDto> {
  UserDto!: FullUserDto;
}
