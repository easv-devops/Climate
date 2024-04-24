import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {Subject, takeUntil} from "rxjs";
import {Device} from "../../../../models/Entities";
import {ClientWantsToGetDeviceByIdDto} from "../../../../models/ClientWantsToGetDeviceByIdDto";

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
              private ws: WebSocketConnectionService) {
  }

  ngOnInit() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
    this.getDevice(this.idFromRoute!);

    // Subscribe to device observable
    this.ws.device.pipe(
      takeUntil(this.unsubscribe$)
    ).subscribe(d => {
      if (d) {
        this.device = d;
      }
    });
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  getDevice(id: number) {
    var dto = new ClientWantsToGetDeviceByIdDto({
      DeviceId: id
    });
    this.ws.socketConnection.sendDto(dto)
  }

}
