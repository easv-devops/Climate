
//Returned data types from backend
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
