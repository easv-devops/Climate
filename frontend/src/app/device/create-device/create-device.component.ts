import {Component} from '@angular/core';
import {FormBuilder, Validators} from '@angular/forms';
import {ClientWantsToCreateDeviceDto} from "../../../models/ClientWantsToCreateDeviceDto";
import {DeviceService} from "../device.service";
import {ToastController} from "@ionic/angular";

@Component({
  selector: 'app-create-device',
  templateUrl: './create-device.component.html',
  styleUrls: ['./create-device.component.scss'],
})

export class CreateDeviceComponent {

  readonly form = this.fb.group({
    deviceName: ['', Validators.required],
    roomId: ['', Validators.required]
  });

  constructor(
      private readonly fb: FormBuilder,
      private deviceService: DeviceService,
      private toastController: ToastController // ToastController injected here
  ) {
  }


  get deviceName() {
    return this.form.controls.deviceName;
  }

  get roomId() {
    return this.form.controls.roomId
  }

  async createDevice() {
    let device = new ClientWantsToCreateDeviceDto({
      DeviceName: this.deviceName.value!,
      RoomId: 1 // Hardcoded value for roomId
    });

    try {
      this.deviceService.createDevice(device);
      const toast = await this.toastController.create({
        message: 'Device successfully created',
        duration: 2000, // Toast duration in milliseconds
        position: 'bottom', // Toast position
        color: "success"
      });
      await toast.present();
    } catch (error) {
      const toast = await this.toastController.create({
        message: 'Failed to create device',
        duration: 2000, // Toast duration in milliseconds
        position: 'bottom', // Toast position
        color: 'danger'
      });
      await toast.present();
    }
  }
}
