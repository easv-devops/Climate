import { Component, OnInit } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {Subject, takeUntil} from "rxjs";
import {ActivatedRoute, Router} from "@angular/router";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {Room} from "../../../../models/room";
import {RoomService} from "../room.service";

@Component({
  selector: 'app-edit-room',
  templateUrl: './edit-room.component.html'
})
export class EditRoomComponent  implements OnInit {
  readonly form = this.fb.group({
    roomName: ['', Validators.required],
    roomId: ['', Validators.required]
  });
  idFromRoute?: number;
  private unsubscribe$ = new Subject<void>();
  specificRoom?: Room;


  constructor(private readonly fb: FormBuilder,
              private readonly roomService : RoomService,
              private readonly activatedRoute: ActivatedRoute,
              public ws: WebSocketConnectionService,
              private readonly router: Router) { }

  ngOnInit() {
    this.getRoomFromRoute();
    this.subscribeToRoom();
    this.subscribeToIsRoomEdited();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  get roomName() {
    return this.form.controls.roomName;
  }

  get roomId() {
    return this.form.controls.roomId;
  }

  getRoomFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
  }

  editRoom() {
    this.roomService.editRoom(this.roomName.value!, this.idFromRoute!);
    this.router.navigate(['/rooms/all']);
  }

  subscribeToRoom() {
    this.ws.allRooms
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(allRooms => {
        if (allRooms) {
          this.specificRoom = allRooms[this.idFromRoute!]
        }
      });
  }

  subscribeToIsRoomEdited() {
    this.roomService.isRoomEdited()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(isEdited => {
        if (isEdited === true) {
          this.roomService.updateRoom(this.specificRoom!)
          this.router.navigate(['/rooms/' + this.idFromRoute]);
          this.ws.setIsRoomEdited(false);
        }
      });
  }
}
