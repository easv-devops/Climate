import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {Subject, takeUntil} from "rxjs";
import {FullUserDto} from "../../../models/ServerSendsUser";
import {PopoverController} from "@ionic/angular";

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['../home.page.scss'],
})
export class TopbarComponent implements OnInit {
  private unsubscribe$ = new Subject<void>();
  user!: FullUserDto;

  constructor(private router: Router,
              private ws: WebSocketConnectionService,
              private popoverController: PopoverController) {
  }

  ngOnInit() {
    this.subscribeToUsers();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  subscribeToUsers() {
    this.ws.user
      .pipe(
        takeUntil(this.unsubscribe$)
      )
      .subscribe(user => {
        if (user !== undefined) {
          this.user = user;
        }
      });
  }

  goToUserSettings() {
    this.popoverController.dismiss();
    this.router.navigate(['/user']);
  }

  signOut() {
    this.popoverController.dismiss();
    this.router.navigateByUrl("/auth/login");
    this.ws.clearDataOnLogout();
  }
}
