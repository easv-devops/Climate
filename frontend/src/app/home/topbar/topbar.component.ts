import {Component} from '@angular/core';
import {Router} from "@angular/router";
import {WebSocketConnectionService} from "../../web-socket-connection.service";

@Component({
  selector: 'app-topbar',
  templateUrl: './topbar.component.html',
  styleUrls: ['../home.page.scss'],
})
export class TopbarComponent {

  constructor(private router: Router,
              private ws: WebSocketConnectionService) {
  }

  signOut() {
    this.router.navigateByUrl("/auth/login");
    this.ws.clearDataOnLogout();
  }
}
