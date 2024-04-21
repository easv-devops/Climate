
//Returned data types from backend
export interface ServerAuthenticatesUserDto {
  Jwt: string;
}

export class User {
  firstname!: string;
  lastname!: string;
  email!: string;
}
