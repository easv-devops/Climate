import {Component, Input, OnInit} from '@angular/core';
import {ClientWantsToDeleteDevice} from "../../../../models/clientRequests";
import {DeviceService} from "../../devices/device.service";
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {DeviceRange} from "../../../../models/Entities";

@Component({
  selector: 'app-device-settings',
  templateUrl: './device-settings.component.html',
  styleUrls: ['./device-settings.component.scss'],
})
export class DeviceSettingsComponent  implements OnInit {
  @Input() deviceId!: number;
  private unsubscribe$ = new Subject<void>();
  public deviceRange!: DeviceRange

  // Bind værdier til lokale variabler
  temperatureRange = { lower: 0, upper: 40 };
  humidityRange = { lower: 0, upper: 100 };
  particle25Max = 0;
  particle100Max = 0;

  constructor(public deviceService: DeviceService,  private ws: WebSocketConnectionService) { }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  ngOnInit() {
    this.deviceService.getDeviceSettingsForDevice(this.deviceId);
    this.subscribeToDevice();
  }

  deleteDevice() {

  }

  subscribeToDevice() {
    this.ws.allDeviceSettings
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(allDevices => {
        if (allDevices && allDevices[this.deviceId]) {
          // Update deviceRange and print new settings
          this.deviceRange = allDevices[this.deviceId];
          this.temperatureRange = { lower: this.deviceRange.TemperatureMin, upper: this.deviceRange.TemperatureMax };
          this.humidityRange = { lower: this.deviceRange.HumidityMin, upper: this.deviceRange.HumidityMax };
          this.particle25Max = this.deviceRange.Particle25Max;
          this.particle100Max = this.deviceRange.Particle100Max;
        }
      });
  }

  createDeviceReading(): DeviceRange {
    return {
      DeviceId: this.deviceId,
      TemperatureMin: this.temperatureRange.lower,
      TemperatureMax: this.temperatureRange.upper,
      HumidityMin: this.humidityRange.lower,
      HumidityMax: this.humidityRange.upper,
      Particle25Max: this.particle25Max,
      Particle100Max: this.particle100Max
    };
  }

  EditDeviceRange() {
    this.deviceService.editDeviceRange(this.createDeviceReading())
  }
}

//todo method for edit device range settings (should be triggered when one of the device range values is changed)
