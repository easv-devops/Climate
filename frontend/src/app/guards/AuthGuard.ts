import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import {WebSocketConnectionService} from "../web-socket-connection.service";
import {ToastController} from "@ionic/angular";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor( private router: Router,
               private ws: WebSocketConnectionService,
               private toast: ToastController) {}

  async canActivate() {
    // Tjek om brugeren har en gyldig JWT
    if (this.isLoggedIn()) {
      return true; // Tillad adgang til ruten, hvis brugeren er logget ind

    } else {
      await (await this.toast.create({
        message: "Please login to access the requested URL",
        color: "warning",
        duration: 5000
      })).present();

      // Hvis brugeren ikke er logget ind, omdiriger til login-siden
      this.router.navigate(['/auth/login']);
      return false; // Bloker adgang til ruten
    }
  }

  isLoggedIn(): boolean {
    const currentJwt = this.ws.jwtSubject.getValue();
    return currentJwt !== undefined && currentJwt !== '';

  }
}
