import {Component, Input, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {ClientWantsToGetDeviceIdsForRoomDto} from "../../../../../models/ClientWantsToGetDeviceIdsForRoomDto";
import {WebSocketConnectionService} from "../../../../web-socket-connection.service";
import {ActivatedRoute, Router} from "@angular/router";
import {Device} from "../../../../../models/Entities";

@Component({
  selector: 'app-device-card',
  templateUrl: './device-card.component.html',
  styleUrls: ['./device-card.component.scss'],
})

export class RoomCardComponent implements OnInit {
  @Input() deviceId: number | undefined;
  idFromRoute: number | undefined;
  private unsubscribe$ = new Subject<void>();

  public device!: Device;

  constructor(private ws: WebSocketConnectionService,
              private router: Router) { }

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

  ngOnInit(): void {
    this.subscribeToRoomDevice();
  }

  routeToDevice() {
    this.router.navigate(['/devices/' + this.deviceId]);
  }
}
