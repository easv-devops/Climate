import {BaseDto} from "./baseDto";
//todo maybe we should refactor when more calls are added, so it dont get clustered
export class ClientWantsToRegisterDto extends BaseDto<ClientWantsToRegisterDto>{
  Email?: string;
  CountryCode?: string;
  Phone?: string;
  FirstName?: string;
  LastName?: string;
  Password?: string;
}

export class ClientWantsToCreateRoom extends BaseDto<ClientWantsToCreateRoom>{
  RoomName?: string;
}

export class ClientWantsAllRooms extends BaseDto<ClientWantsAllRooms>{
}

export class ClientWantsSpecificRoom extends BaseDto<ClientWantsSpecificRoom>{
  RoomId!: number;
}

export class ClientWantsToDeleteRoom extends BaseDto<ClientWantsToDeleteRoom>{
  RoomId!: number;
}

export class ClientWantsToEditRoom extends BaseDto<ClientWantsToEditRoom>{
  RoomId!: number;
  RoomName!: string;
}

export class ClientWantsToAuthenticate extends BaseDto<ClientWantsToAuthenticate> {
  Email?: string;
  Password?: string;
}
//todo make when time not something we need but could be nice
//should call endpoint for just logging a user in, if token is not expired in backend.
export class ClientWantsToAuthenticateWithJwt extends BaseDto<ClientWantsToAuthenticateWithJwt> {
  jwt?: string;
}

export class ClientWantsToResetPassword extends BaseDto<ClientWantsToResetPassword> {
  Email?: string;
}

export class ClientWantsToDeleteDevice extends BaseDto<ClientWantsToDeleteDevice> {
  Id?: number;
}
