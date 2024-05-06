import {Component, OnInit, ViewChild} from '@angular/core';
import {Room} from "../../../../models/room";
import {IonModal} from "@ionic/angular";
import {RoomService} from "../room.service";
import {FormBuilder, Validators} from "@angular/forms";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {Subject, takeUntil} from "rxjs";
import {Router} from "@angular/router";

@Component({
  selector: 'app-all-rooms',
  templateUrl: './all-rooms.component.html',
  styleUrls: ['../rooms.component.scss'],
})
export class AllRoomsComponent implements OnInit {

  private unsubscribe$ = new Subject<void>();
  allRooms: Room[] = [];
  roomname!: string;

  form = this.fb.group({
    roomName: ['', [Validators.required, Validators.minLength(2)]]
  });
  constructor(private roomService: RoomService,
              private readonly fb: FormBuilder,
              private ws: WebSocketConnectionService,
              private router: Router
              ) {}

  ngOnInit() {
    this.roomService.getAllRooms();
    this.subscribeToRooms();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  subscribeToRooms() {
    this.ws.allRooms
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(allRooms => {
        if (!allRooms)
          throw new Error();

        const roomList = Object.values(allRooms!)
        this.allRooms = roomList!;
      });
  }
  @ViewChild(IonModal) modal!: IonModal;

  goToCreateRoom(){
    this.router.navigate(['/rooms/create']);
  }
}
