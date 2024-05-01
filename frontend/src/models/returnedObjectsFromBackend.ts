
//Returned data types from backend
import {Room} from "./room";

export class ServerAuthenticatesUserDto {
  Jwt!: string;
}

export class User {
  firstname!: string;
  lastname!: string;
  email!: string;
}

export class ServerRegisterUserDto {
  Jwt!: string;
}

export class ServerResetsPasswordDto {
  IsReset!: boolean;
}

export class ServerReturnsAllRoomsDto{
  rooms?: Room[];
}

export class ServerSendsErrorMessageToClient {
  errorMessage!: string;
  receivedMessage!: string
}
