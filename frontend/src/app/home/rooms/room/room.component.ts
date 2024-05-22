import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Subject, takeUntil} from "rxjs";
import {Device, Room} from "../../../../models/Entities";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {RoomService} from "../room.service";
import {ClientWantsToGetDeviceIdsForRoomDto} from "../../../../models/ClientWantsToGetDeviceIdsForRoomDto";
import {AlertController} from "@ionic/angular";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.scss'],
})
export class RoomComponent implements OnInit {
  idFromRoute: number | undefined;
  private unsubscribe$ = new Subject<void>();

  room!: Room
  devices: Device[] = [];


  constructor(private activatedRoute: ActivatedRoute,
              private roomService: RoomService,
              private ws: WebSocketConnectionService,
              private readonly router: Router,
              private alertController: AlertController) {
  }

  ngOnInit() {

    this.getRoomFromRoute();//todo skal bruges til at loade room info later
    this.subscribeToRoomDevice();//todo skal ændres til allrooms.
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  getRoomFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
  }


  subscribeToRoomDevice() {
    this.ws.socketConnection.sendDto(new ClientWantsToGetDeviceIdsForRoomDto({RoomId: this.idFromRoute}));
    this.ws.allRooms
      .pipe(
        takeUntil(this.unsubscribe$)
      )
      .subscribe(roomRecord => {
        if (roomRecord !== undefined) {
          if (roomRecord[this.idFromRoute!].DeviceIds === undefined) {
            this.ws.socketConnection.sendDto(new ClientWantsToGetDeviceIdsForRoomDto({RoomId: this.idFromRoute}))
          }
          const selectedRoom = roomRecord[this.idFromRoute!];
          //checks if any changes in room from server and updates room and devices
          this.room = selectedRoom;
          this.subscribeToDevice();
        }
      });
  }

  subscribeToDevice() {
    this.ws.allDevices
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(allDevices => {
        if (allDevices !== undefined) {
          this.devices = [];

          for (const deviceId of this.room.DeviceIds ?? []) {
            let device = allDevices[deviceId];

            if (device) {
              this.devices.push(device);
            }
          }
        }
      });
  }


  deleteRoom() {
    this.roomService.deleteRoom(this.idFromRoute as number)
    this.router.navigate(["rooms/all"])
  }

  async presentDeleteRoomAlert() {
    let deviceNames = this.devices.map(device => device.DeviceName).join(', ');

    if (!deviceNames) {
      deviceNames = 'No devices found.';
    }

    const alert = await this.alertController.create({
      header: 'Are you sure you want to delete ' + this.room.RoomName + '?',
      message: 'Deleting this room will delete all associated devices and their readings: ' + deviceNames,
      buttons: [
        {
          text: 'No',
          role: 'cancel'
        },
        {
          text: 'Yes',
          handler: () => {
            this.deleteRoom();
          }
        }
      ]
    });
    await alert.present();
  }

}
