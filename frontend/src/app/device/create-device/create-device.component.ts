import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ClientWantsToCreateDeviceDto } from "../../../models/ClientWantsToCreateDeviceDto";
import { DeviceService } from "../device.service";
import { ToastController } from "@ionic/angular";
import { WebSocketConnectionService } from "../../web-socket-connection.service";
import { Router } from "@angular/router";
import { Subject, takeUntil } from "rxjs";

@Component({
  selector: 'app-create-device',
  templateUrl: './create-device.component.html',
  styleUrls: ['./create-device.component.scss'],
})
export class CreateDeviceComponent implements OnInit, OnDestroy {

  readonly form = this.fb.group({
    deviceName: ['', Validators.required],
    roomId: ['', Validators.required]
  });

  private unsubscribe$ = new Subject<void>();

  constructor(
      private readonly fb: FormBuilder,
      private deviceService: DeviceService,
      private toastController: ToastController,
      public ws: WebSocketConnectionService,
      private router: Router
  ) { }

  ngOnInit(): void {
    this.ws.deviceId.pipe(
        takeUntil(this.unsubscribe$)
    ).subscribe(deviceId => {
      if (deviceId) {
        this.router.navigate(['http://localhost:4200/#/devices/' + deviceId]);
        console.log(deviceId);
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  get deviceName() {
    return this.form.controls.deviceName;
  }

  get roomId() {
    return this.form.controls.roomId;
  }

  async createDevice() {
    let device = new ClientWantsToCreateDeviceDto({
      DeviceName: this.deviceName.value!,
      RoomId: 1 // Hardcoded value for roomId
    });
    this.deviceService.createDevice(device);
  }
}
