import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {RoomService} from "../room.service";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {Subject, takeUntil} from "rxjs";
import {Room} from "../../../../models/Entities";

@Component({
  selector: 'app-all-rooms',
  templateUrl: './all-rooms.component.html',
  styleUrls: ['./all-rooms.component.scss'],
})
export class AllRoomsComponent implements OnInit {
  private unsubscribe$ = new Subject<void>();
  public listOfRooms!: number[]
  allRooms?: Room[];

  constructor(private router: Router,
              private ws: WebSocketConnectionService) {
  }

  ngOnInit() {
    this.subscribeToAllRooms();
    this.checkIfUserHasRooms();
  }


  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  //todo load a list of all deviceId into all rooms observable
  subscribeToAllRooms() {
    this.ws.allRooms
      .pipe(
        takeUntil(this.unsubscribe$)
      )
      .subscribe(roomRecord => {
        if (roomRecord !== undefined) {
          this.allRooms = Object.values(roomRecord);
        }
      });
  }

  checkIfUserHasRooms() {
    if (!this.allRooms || this.allRooms.length === 0) {
      this.router.navigate(['/landing']);
    }
  }

}
