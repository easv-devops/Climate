import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DeviceService } from "../../devices/device.service";
import { WebSocketConnectionService } from "../../../web-socket-connection.service";
import { DeviceRange, DeviceSettings } from "../../../../models/Entities";

@Component({
  selector: 'app-device-settings',
  templateUrl: './device-settings.component.html',
  styleUrls: ['./device-settings.component.scss'],
})
export class DeviceSettingsComponent implements OnInit, OnDestroy {
  @Input() deviceId!: number;
  private unsubscribe$ = new Subject<void>();
  public deviceRange!: DeviceRange;
  public advancedSettings!: DeviceSettings;

  settingsForm: FormGroup = this.fb.group({
    TemperatureInterval: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
    ParticleInterval: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
    DeviceInterval: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
  });

  temperatureRange = { lower: 0, upper: 40 };
  humidityRange = { lower: 0, upper: 100 };
  particle25Max = 0;
  particle100Max = 0;

  constructor(
    public deviceService: DeviceService,
    private ws: WebSocketConnectionService,
    private fb: FormBuilder
  ) {}

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  ngOnInit() {
    this.deviceService.getDeviceSettingsForDevice(this.deviceId);
    this.subscribeToDevice();
  }

  subscribeToDevice() {
    this.ws.allDeviceRangeSettings
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(allDevices => {
        if (allDevices && allDevices[this.deviceId]) {
          this.deviceRange = allDevices[this.deviceId];
          this.temperatureRange = { lower: this.deviceRange.TemperatureMin, upper: this.deviceRange.TemperatureMax };
          this.humidityRange = { lower: this.deviceRange.HumidityMin, upper: this.deviceRange.HumidityMax };
          this.particle25Max = this.deviceRange.Particle25Max;
          this.particle100Max = this.deviceRange.Particle100Max;
        }
      });

    this.ws.allDeviceSettings
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(allDevices => {
        if (allDevices && allDevices[this.deviceId]) {
          this.advancedSettings = allDevices[this.deviceId];
          this.populateSettingsForm(this.advancedSettings);
        }
      });
  }

  populateSettingsForm(settings: DeviceSettings) {
    this.settingsForm.patchValue({
      TemperatureInterval: settings.BMP280ReadingInterval || '',
      ParticleInterval: settings.PMSReadingInterval || '',
      DeviceInterval: settings.UpdateInterval || '',
    });
  }

  createDeviceRangeReading(): DeviceRange {
    return {
      Id: this.deviceId,
      TemperatureMin: this.temperatureRange.lower,
      TemperatureMax: this.temperatureRange.upper,
      HumidityMin: this.humidityRange.lower,
      HumidityMax: this.humidityRange.upper,
      Particle25Max: this.particle25Max,
      Particle100Max: this.particle100Max
    };
  }

  EditDeviceRange() {
    this.deviceService.editDeviceRange(this.createDeviceRangeReading());
  }

  onSubmit() {
    if (this.settingsForm.valid) {
      const updatedSettings: DeviceSettings = {
        Id: this.deviceId,
        BMP280ReadingInterval: this.settingsForm.value.TemperatureInterval,
        PMSReadingInterval: this.settingsForm.value.ParticleInterval,
        UpdateInterval: this.settingsForm.value.DeviceInterval
      };
      this.deviceService.updateDeviceSettings(updatedSettings);
    }
  }



  deleteDevice() {

  }


}
