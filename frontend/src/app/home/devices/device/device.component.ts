import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Subject, takeUntil} from "rxjs";
import {Device} from "../../../../models/Entities";
import {DeviceService} from "../device.service";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {ClientWantsToDeleteDevice} from "../../../../models/clientRequests";
import {AlertController} from "@ionic/angular";


@Component({
  selector: 'app-device',
  templateUrl: './device.component.html',
  styleUrls: ['./device.component.scss'],
})
export class DeviceComponent implements OnInit {
  idFromRoute: number | undefined;
  device?: Device;
  private unsubscribe$ = new Subject<void>();

  constructor(private router: Router,
              private activatedRoute: ActivatedRoute,
              public ws: WebSocketConnectionService,
              private deviceService: DeviceService,
              private alertController: AlertController) {
  }

  ngOnInit() {
    this.getDeviceFromRoute();
    this.subscribeToDevice();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  getDeviceFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
  }

  subscribeToDevice() {
    this.ws.allDevices
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(allDevices => {
        if (allDevices) {
          this.device = allDevices[this.idFromRoute!]
        }
      });
  }

  get deviceId() {
    return this.device?.Id;
  }

  deleteDevice() {
    const deviceIdToDelete = this.deviceId;

    let deviceToDelete = new ClientWantsToDeleteDevice({
      Id: deviceIdToDelete,
    });
    this.deviceService.deleteDevice(deviceToDelete);

    const roomId = this.device?.RoomId;

    if (roomId) {
      this.router.navigate(['/rooms', roomId]);
    } else {
      console.error('Room ID not found for the device');
    }
  }


  async presentDeleteDeviceAlert() {
    const alert = await this.alertController.create({
      header: 'Are you sure you want to delete ' + this.device?.DeviceName + '?',
      buttons: [
        {
          text: 'No',
          role: 'cancel'
        },
        {
          text: 'Yes',
          handler: () => {
            this.deleteDevice();
          }
        }
      ]
    });
    await alert.present();
  }


}
