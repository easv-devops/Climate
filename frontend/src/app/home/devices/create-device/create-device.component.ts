import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, Validators} from '@angular/forms';
import {ClientWantsToCreateDeviceDto} from "../../../../models/ClientWantsToCreateDeviceDto";
import {DeviceService} from "../device.service";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {Router} from "@angular/router";
import {Subject, takeUntil} from "rxjs";
import {Room} from "../../../../models/Entities";

@Component({
  selector: 'app-create-device',
  templateUrl: './create-device.component.html',
  styleUrls: ['./create-device.component.scss'],
})
export class CreateDeviceComponent implements OnInit, OnDestroy {

  readonly form = this.fb.group({
    deviceName: ['', Validators.required],
    roomId: ['', Validators.required],
  });

  private unsubscribe$ = new Subject<void>();
  public allRooms: Room[] | undefined;
  public selectedRoomId?: number;

  constructor(
    private readonly fb: FormBuilder,
    private deviceService: DeviceService,
    public ws: WebSocketConnectionService,
    private router: Router
  ) {
  }


  ngOnInit(): void {
    this.subscribeToRooms()
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  get deviceName() {
    return this.form.controls.deviceName;
  }

  createDevice() {
    this.router.navigate(['rooms/'+this.selectedRoomId])

    let device = new ClientWantsToCreateDeviceDto({
      DeviceName: this.deviceName.value!,
      //TODO Read real RoomId value from room
      RoomId: this.selectedRoomId // Hardcoded value for roomId
    });
    this.deviceService.createDevice(device);
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
