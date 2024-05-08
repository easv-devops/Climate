import {Component, Input, OnInit} from '@angular/core';
import {Room} from "../../../../models/Entities";
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";

@Component({
  selector: 'app-room-card',
  templateUrl: './room-card.component.html',
  styleUrls: ['./room-card.component.scss'],
})
export class RoomCardComponent  implements OnInit {
  @Input() roomId!: number;
  room: Room | undefined;
  private unsubscribe$ = new Subject<void>();

  constructor(private ws: WebSocketConnectionService) { }

  ngOnInit() {
    if(this.roomId !== -1) {
      this.subscribeToRoom();
    } else {
      this.newRoomButton();
    }
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  subscribeToRoom() {
    this.ws.allRooms
      .pipe(
        takeUntil(this.unsubscribe$)
      )
      .subscribe(roomRecord => {
        if (roomRecord !== undefined) {
          this.room = roomRecord[this.roomId];
        }
      });
  }

  private newRoomButton() {
    this.room = new Room();
    this.room.RoomName = "New Room";
  }
}
