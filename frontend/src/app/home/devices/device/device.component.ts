import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Subject, takeUntil} from "rxjs";
import {Device} from "../../../../models/Entities";
import {DeviceService} from "../device.service";

@Component({
  selector: 'app-device',
  templateUrl: './device.component.html',
  styleUrls: ['./device.component.scss'],
})
export class DeviceComponent implements OnInit {
  idFromRoute: number | undefined;
  device?: Device;
  private unsubscribe$ = new Subject<void>();

  constructor(private activatedRoute: ActivatedRoute,
              private deviceService: DeviceService) {
  }

  ngOnInit() {
    this.getDeviceFromRoute();
    this.subscribeToDevice()
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  getDeviceFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
    this.deviceService.getDeviceById(this.idFromRoute)
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
}
