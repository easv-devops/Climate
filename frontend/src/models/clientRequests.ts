import {BaseDto} from "./baseDto";

export class ClientWantsToRegister extends BaseDto<ClientWantsToRegister>{
  Email?: string;
  Phone?: string;
  Name?: string;
  Password?: string;
}

export class ClientWantsToAuthenticate extends BaseDto<ClientWantsToAuthenticate> {
  Email?: string;
  Password?: string;
}


export class ClientWantsToAuthenticateWithJwt extends BaseDto<ClientWantsToAuthenticateWithJwt> {
  jwt?: string;
}
