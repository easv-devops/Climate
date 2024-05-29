import { Component, OnInit } from '@angular/core';
import {Router} from "@angular/router";
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../web-socket-connection.service";

@Component({
  selector: 'app-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.scss'],
})
export class LandingPageComponent  implements OnInit {
  private unsubscribe$ = new Subject<void>();

  constructor(private router: Router,
              private ws: WebSocketConnectionService) { }

  ngOnInit() {
    this.subscribeToAllRooms();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  routeToCreateRoom() {
    this.router.navigate(["rooms/add"])
  }

  subscribeToAllRooms() {
    this.ws.allRooms
      .pipe(
        takeUntil(this.unsubscribe$)
      )
      .subscribe(roomRecord => {
        if (roomRecord !== undefined) {
          this.router.navigate(["rooms/all"]);
        }
      });
  }
}
