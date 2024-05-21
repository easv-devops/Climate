import {Component, OnInit} from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {RoomService} from "../room.service";
import {Router} from "@angular/router";
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";

@Component({
  selector: 'app-create-room',
  templateUrl: './create-room.component.html',
  styleUrls: ['./create-room.component.scss'],
})
export class CreateRoomComponent implements OnInit {

  readonly form = this.fb.group({
    roomName: ['', Validators.required]
  });

  private unsubscribe$ = new Subject<void>();

  constructor(private readonly fb: FormBuilder,
              private readonly roomService: RoomService,
              private readonly router: Router,
              private readonly ws: WebSocketConnectionService) {}

  ngOnInit() {
    this.subscribeToAllRooms();
  }

  get roomName() {
    return this.form.controls.roomName;
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  createRoom() {
    this.roomService.createRoom(this.roomName.value!)
  }

  subscribeToAllRooms() {
    this.ws.allRooms
      .pipe(
        takeUntil(this.unsubscribe$)
      )
      .subscribe(roomRecord => {
        if (roomRecord !== undefined && Object.keys(roomRecord).length > 0) {
          this.router.navigate(["rooms/all"]);
        }
      });
  }


}
