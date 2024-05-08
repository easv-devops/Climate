export class Device {
  Id!: number;
  RoomId!: number;
  DeviceName!: string;
}

export class DeviceInRoom {
  Id!: number;
  DeviceName!: string;
}

export class SensorDto {
  Value!: number;
  TimeStamp!: Date | number;
}

export class Room {
  Id!: number;
  RoomName!: string;
  DeviceIds?: number[];
}
