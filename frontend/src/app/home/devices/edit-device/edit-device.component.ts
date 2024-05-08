import { Component, OnInit } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {DeviceService} from "../device.service";
import {ActivatedRoute, Router} from "@angular/router";
import {ClientWantsToEditDeviceDto} from "../../../../models/ClientWantsToEditDeviceDto";
import {Subject, takeUntil} from "rxjs";
import {Device, Room} from "../../../../models/Entities";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";

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

  public allRooms: Room[] | undefined;
  public selectedRoomId?: number;



  constructor(private readonly fb: FormBuilder,
              private readonly deviceService: DeviceService,
              private readonly activatedRoute: ActivatedRoute,
              public ws: WebSocketConnectionService,
              private readonly router: Router) { }

  ngOnInit() {
    this.getDeviceFromRoute();
    this.subscribeToDevice();
    this.subscribeToIsDeviceEdited();
    this.subscribeToRooms();
    this.selectedRoomId = this.device?.RoomId
  }

  ngOnDestroy() {
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
  }

  editDevice() {
    let dto = new ClientWantsToEditDeviceDto({
      Id: this.idFromRoute!,
      DeviceName: this.deviceName.value!,
      RoomId: this.selectedRoomId
    });
    this.deviceService.editDevice(dto);
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

  subscribeToIsDeviceEdited() {
    this.deviceService.isDeviceEdited()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(isEdited => {
        if (isEdited === true) {
          this.router.navigate(['/devices/' + this.idFromRoute]);
          this.ws.setIsDeviceEdited(false);
        }
      });
  }

  private subscribeToRooms() {
    this.ws.allRooms
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(roomRecord => {
        if (roomRecord !== undefined) {
          // Hent alle enheder fra recordet
          this.allRooms = Object.values(roomRecord);
        }
      });
  }

  onRoomSelectionChange(event: CustomEvent) {
    // Gem den valgte værdi, når der sker ændringer i ion-select
    this.selectedRoomId = event.detail.value;
  }


}
