import {BaseDto} from "./baseDto";

export class ServerSendsUser extends BaseDto<ServerSendsUser>  {
  UserDto!: FullUserDto;
}

export class FullUserDto {
  Id!: number;
  Email!: string;
  FirstName!: string;
  LastName!: string;
  CountryCode!: string;
  Number!: string;
}
