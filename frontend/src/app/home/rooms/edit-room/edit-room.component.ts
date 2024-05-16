import { Component, OnInit } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {RoomService} from "../room.service";
import {ActivatedRoute, Router} from "@angular/router";
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {Room} from "../../../../models/Entities";

@Component({
  selector: 'app-edit-room',
  templateUrl: './edit-room.component.html',
  styleUrls: ['./edit-room.component.scss'],
})
export class EditRoomComponent  implements OnInit {
  private unsubscribe$ = new Subject<void>();
  room: Room | undefined;

  readonly form = this.fb.group({
    roomName: ['', Validators.required]
  });
  private idFromRoute?: number;

  get roomName() {
    return this.form.controls.roomName;
  }

  constructor(private readonly fb: FormBuilder,
              private readonly roomService: RoomService,
              private activatedRoute: ActivatedRoute,
              private readonly router: Router,
              private ws: WebSocketConnectionService) { }

  ngOnInit() {
    this.getRoomFromRoute();
    this.subscribeToRoom();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  subscribeToRoom() {
    this.ws.allRooms
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(allRooms => {
        if (allRooms) {
          this.room = allRooms[this.idFromRoute!]

          this.form.patchValue({
            roomName: this.room.RoomName
          });
        }
      });
  }

  editRoom() {
    this.roomService.editRoom(this.idFromRoute!, this.roomName.value!);
    this.router.navigate(["rooms/"+this.idFromRoute]);
  }

  getRoomFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
  }
}
