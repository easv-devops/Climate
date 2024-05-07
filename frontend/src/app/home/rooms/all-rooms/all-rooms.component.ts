import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {RoomService} from "../room/room.service";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {Subject, takeUntil} from "rxjs";
import {ClientWantsToGetDeviceIdsForRoomDto} from "../../../../models/ClientWantsToGetDeviceIdsForRoomDto";

@Component({
  selector: 'app-all-rooms',
  templateUrl: './all-rooms.component.html',
  styleUrls: ['./all-rooms.component.scss'],
})
export class AllRoomsComponent implements OnInit {

  private unsubscribe$ = new Subject<void>();
  public listOfRooms!: number[]

  constructor(private activatedRoute: ActivatedRoute,
              private roomService: RoomService,
              private ws: WebSocketConnectionService) {
  }

  ngOnInit() {

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

      }
      });
  }

}
