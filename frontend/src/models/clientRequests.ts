import {BaseDto} from "./baseDto";

export class ClientWantsToRegisterDto extends BaseDto<ClientWantsToRegisterDto>{
  Email?: string;
  CountryCode?: string;
  Phone?: string;
  FirstName?: string;
  LastName?: string;
  Password?: string;
}

export class ClientWantsToAuthenticate extends BaseDto<ClientWantsToAuthenticate> {
  Email?: string;
  Password?: string;
}

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
