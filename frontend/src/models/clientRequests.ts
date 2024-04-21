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


export class ClientWantsToAuthenticateWithJwt extends BaseDto<ClientWantsToAuthenticateWithJwt> {
  jwt?: string;
}

