
//Returned data types from backend
export class ServerAuthenticatesUserDto {
  Jwt!: string;
}

export class ServerRegisterUserDto {
  Jwt!: string;
}

export class ServerResetsPasswordDto {
  IsReset!: boolean;
}
export class ServerSendsErrorMessageToClient {
  errorMessage!: string;
  receivedMessage!: string
}

export class DeviceWithIdDto {
  DeviceName!: string;
  RoomId!: number;
  Id!: number;
}

export class ServerSendsDeviceDeletionStatusDto {
  IsDeleted!: boolean;
  Id!: number;
}
