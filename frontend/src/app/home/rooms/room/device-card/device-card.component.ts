import {Component, Input, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../../../web-socket-connection.service";
import {Router} from "@angular/router";
import {Device} from "../../../../../models/Entities";

@Component({
  selector: 'app-device-card',
  templateUrl: './device-card.component.html',
  styleUrls: ['./device-card.component.scss'],
})

export class DeviceCardComponent implements OnInit {
  @Input() deviceId: number | undefined;
  private unsubscribe$ = new Subject<void>();

  public device!: Device;

  constructor(private ws: WebSocketConnectionService,
              private router: Router) { }

  ngOnInit(): void {
    this.subscribeToRoomDevice();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
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

  routeToDevice() {
    this.router.navigate(['/devices/' + this.deviceId]);
  }
}
