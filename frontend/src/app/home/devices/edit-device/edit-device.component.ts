import { Component, OnInit } from '@angular/core';
import {ClientWantsToCreateDeviceDto} from "../../../../models/ClientWantsToCreateDeviceDto";
import {FormBuilder, Validators} from "@angular/forms";
import {DeviceService} from "../device.service";
import {ActivatedRoute, Router} from "@angular/router";
import {ClientWantsToEditDeviceDto} from "../../../../models/ClientWantsToEditDeviceDto";
import {Subject, takeUntil} from "rxjs";
import {Device} from "../../../../models/Entities";

@Component({
  selector: 'app-edit-device',
  templateUrl: './edit-device.component.html',
  styleUrls: ['./edit-device.component.scss'],
})
export class EditDeviceComponent  implements OnInit {
  readonly form = this.fb.group({
    deviceName: ['', Validators.required],
    roomId: ['', Validators.required]
  });
  idFromRoute?: number;
  private unsubscribe$ = new Subject<void>();
  isEdited?: boolean;
  device?: Device;


  constructor(private readonly fb: FormBuilder,
              private readonly deviceService: DeviceService,
              private readonly activatedRoute: ActivatedRoute,
              private readonly router: Router) { }

  ngOnInit() {
    this.getDeviceFromRoute();
    this.subscribeToDevice();
    this.subscribeToIsDeviceEdited();
  }

  ngOnDestroy() {
    console.log('ngOnDestroy fired fra edit-device');
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  get deviceName() {
    return this.form.controls.deviceName;
  }

  get roomId() {
    return this.form.controls.roomId;
  }

  getDeviceFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
    this.deviceService.getDeviceById(this.idFromRoute)
  }

  editDevice() {
    let dto = new ClientWantsToEditDeviceDto({
      Id: this.idFromRoute!,
      DeviceName: this.deviceName.value!,
      RoomId: 1, //TODO Fix when rooms is ready
    });
    this.deviceService.editDevice(dto);
  }

  subscribeToDevice() {
    this.deviceService.getDeviceObservable()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(d => {
        if (d) {
          this.device = d;
        }
      });
  }

  subscribeToIsDeviceEdited() {
    this.deviceService.isDeviceEdited()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(isEdited => {
        if (isEdited) {
          this.deviceService.updateDevice(this.device!)
          this.router.navigate(['/devices/' + this.idFromRoute]);
          this.isEdited = false;
        }
      });
  }
}
