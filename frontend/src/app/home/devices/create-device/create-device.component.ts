import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {ClientWantsToCreateDeviceDto} from "../../../../models/ClientWantsToCreateDeviceDto";
import {DeviceService} from "../device.service";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {Subject, takeUntil} from "rxjs";

@Component({
  selector: 'app-create-device',
  templateUrl: './create-device.component.html',
  styleUrls: ['./create-device.component.scss'],
})
export class CreateDeviceComponent implements OnInit, OnDestroy {

  readonly form = this.fb.group({
    deviceName: ['', Validators.required],
  });

  private unsubscribe$ = new Subject<void>();
  roomId?: number;
  private idFromRoute?: number;

  constructor(
    private readonly fb: FormBuilder,
    private deviceService: DeviceService,
    public ws: WebSocketConnectionService,
    private router: Router,
    private activatedRoute: ActivatedRoute

  ) { }

  ngOnInit(): void {
    this.getRoomFromRoute();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  get deviceName() {
    return this.form.controls.deviceName;
  }

  createDevice() {
    if (!this.idFromRoute) {
      console.error('RoomId is not available.');
      return;
    }

    this.ws.deviceId.pipe(
      takeUntil(this.unsubscribe$)
    ).subscribe(deviceId => {
      if (deviceId) {
        this.router.navigate(['/devices/' + deviceId]);
      }
    });

    let device = new ClientWantsToCreateDeviceDto({
      DeviceName: this.deviceName.value!,
      RoomId: this.idFromRoute
    });
    this.deviceService.createDevice(device);
  }

  getRoomFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
  }
}
