import {Component, Input, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../../../web-socket-connection.service";
import {Router} from "@angular/router";
import {Device, Room} from "../../../../../models/Entities";

@Component({
  selector: 'app-device-card',
  templateUrl: './device-card.component.html',
  styleUrls: ['./device-card.component.scss'],
})

export class DeviceCardComponent implements OnInit {
  @Input() deviceId: number | undefined;
  private unsubscribe$ = new Subject<void>();
  public alerts = 0;

  public device!: Device;

  constructor(private ws: WebSocketConnectionService) { }

  ngOnInit(): void {
    this.subscribeToAlerts()
    if(this.deviceId !== -1) {
      this.subscribeToRoomDevice();
    } else {
      this.newDeviceButton();
    }
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  private subscribeToAlerts() {
    this.ws.alerts
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(alerts => {
        if (alerts) {
          this.alerts = 0;
          alerts.forEach(alert => {

            if (alert.DeviceId === this.deviceId)
            {
              this.alerts++
            }
          });
        }
      });
  }

  subscribeToRoomDevice() {
    this.ws.allDevices
      .pipe(
        takeUntil(this.unsubscribe$)
      )
      .subscribe(deviceRecord => {
        if (deviceRecord !== undefined) {
          this.device = deviceRecord[this.deviceId!]
        }
      });
  }

  private newDeviceButton() {
    this.device = new Device();
    this.device.DeviceName = "New Device";
  }
}
