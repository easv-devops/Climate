import {Component, Input, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Device} from "../../../../models/device";
import {Subject, takeUntil} from "rxjs";
import {DeviceInRoom} from "../../../../models/Entities";
import {DeviceService} from "../../devices/device.service";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./../rooms.component.scss'],
})
export class RoomComponent  implements OnInit {
  roomId: number | undefined;
  devices: Device[] = [{deviceId: 1},{deviceId: 2},{deviceId: 3}];
  deviceName?: string;
  roomName?: string;

  public alertButtons = [
    {
      text: 'Cancel',
      role: 'cancel',
      handler: () => {
        console.log('Alert canceled');
      },
    },
    {
      text: 'OK',
      role: 'confirm',
      handler: () => {
        console.log('Alert confirmed');
      },
    },
  ];
  idFromRoute: number | undefined;
  private unsubscribe$ = new Subject<void>();
  roomDevices?: DeviceInRoom[];

  constructor(private activatedRoute: ActivatedRoute,
              private deviceService: DeviceService) { }

  ngOnInit() {
    this.getDevicesFromRoute();
    this.subscribeToRoomDevices();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  getDevicesFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
    this.deviceService.getDevicesByRoomId(this.idFromRoute)
  }

  subscribeToRoomDevices() {
    this.deviceService.getRoomDevicesObservable()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(d => {
        if (d) {
          this.roomDevices = d;
        }
      });
  }


  onWillDismiss($event: any) {
  }
  confirm(){
  }
  cancel() {

  }
}
