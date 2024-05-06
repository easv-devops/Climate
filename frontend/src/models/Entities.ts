export class Device {
  Id!: number;
  RoomId!: number;
  DeviceName!: string;
}

export class DeviceInRoom {
  Id!: number;
  DeviceName!: string;
}

export class Room {
  Id!: number;
  RoomName!: string;
  DeviceIds?: number[];
}
