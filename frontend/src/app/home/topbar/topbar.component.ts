import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {Subject, takeUntil} from "rxjs";
import {FullUserDto} from "../../../models/userModels/ServerSendsUser";
import {PopoverController} from "@ionic/angular";
import {AlertDto} from "../../../models/Entities";

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss'],
})
export class TopbarComponent implements OnInit {
  private unsubscribe$ = new Subject<void>();
  user!: FullUserDto;
  alertCount: number = 0;

  constructor(private router: Router,
              private ws: WebSocketConnectionService,
              private popoverController: PopoverController) {
  }

  ngOnInit() {
    this.subscribeToUsers();
    this.subscribeToAlerts();
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

  private subscribeToAlerts() {
    this.ws.alerts
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(alerts => {
        if (alerts) {
          this.alertCount = 0;
          for (const alert of alerts) {
            if(!alert.IsRead) {
              this.alertCount++;
            }
          }
        }
      });
  }
}
