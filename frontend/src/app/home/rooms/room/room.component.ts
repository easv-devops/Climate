import {Component, Input, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Device} from "../../../../models/device";
import {Subject, takeUntil} from "rxjs";
import {DeviceInRoom} from "../../../../models/Entities";
import {DeviceService} from "../../devices/device.service";
import {RoomService} from "../room.service";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {Room} from "../../../../models/room";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./../rooms.component.scss'],
})
export class RoomComponent  implements OnInit {
  specificRoom!: Room;
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
      text: 'Delete',
      role: 'confirm',
      handler: () => {
        console.log(this.specificRoom.Id)
        this.roomService.deleteRoom(this.specificRoom?.Id!)
        console.log('Alert confirmed');
      },
    },
  ];
  idFromRoute: number | undefined;
  private unsubscribe$ = new Subject<void>();
  roomDevices?: DeviceInRoom[];

  constructor(private activatedRoute: ActivatedRoute,
              private deviceService: DeviceService,
              private roomService: RoomService,
              private ws: WebSocketConnectionService) { }

  ngOnInit() {
    this.getRoomFromRoute()
    this.getDevicesFromRoute();
    this.subscribeToRoomDevices();
    this.subscribeToRooms()
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  getRoomFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
    this.roomService.getRoomById(this.idFromRoute)
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
  subscribeToRooms() {
    this.ws.specificRoom
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(theSpecificRoom => {
        if (!theSpecificRoom)
          throw new Error();
        console.log(theSpecificRoom.Id)
        const room = theSpecificRoom
        this.specificRoom = room!;
      });
  }

  onWillDismiss($event: any) {
  }
  confirm(){
  }
  cancel() {

  }
}
