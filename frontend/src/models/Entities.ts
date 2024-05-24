import {BaseDto} from "./baseDto";

export class Device {
  Id!: number;
  RoomId!: number;
  DeviceName!: string;
}

export class DeviceRangeDto  extends BaseDto<DeviceRangeDto>{
  Settings!: DeviceRange;
}
export class DeviceRange{
  Id!: number;
  TemperatureMax!: number;
  TemperatureMin!: number;
  HumidityMax!: number;
  HumidityMin!: number;
  Particle25Max!: number;
  Particle100Max!: number;
}

export class DeviceSettingDto  extends BaseDto<DeviceSettingDto>{
  Settings!: DeviceSettings;
}
export class DeviceSettings {
  Id!: number;
  BMP280ReadingInterval!: number;
  PMSReadingInterval!: number;
  UpdateInterval!: number;
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

export class CountryCode{
  CountryCode!: string;
  IsoCode!: string;
  Country!: string;
}

export class LatestData {
  Id!: number;
  Data?: LatestReadingsDto;
}

export class LatestReadingsDto {
  Temperature?: SensorDto;
  Humidity?: SensorDto;
  Particle25?: SensorDto;
  Particle100?: SensorDto;
}
