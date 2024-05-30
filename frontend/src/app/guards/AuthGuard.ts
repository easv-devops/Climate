import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import {WebSocketConnectionService} from "../web-socket-connection.service";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private ws: WebSocketConnectionService) {}

  async canActivate(): Promise<boolean> {
    // Tjek om brugeren har en gyldig JWT
    if (await this.checkJwtWithRetry()) {
      return true; // Tillad adgang til ruten, hvis brugeren er logget ind
    } else {
      // Hvis brugeren ikke er logget ind, omdiriger til login-siden
      this.router.navigate(['/auth/login']);
      return false; // Bloker adgang til ruten
    }
  }

  isLoggedIn(): boolean {
    const currentJwt = this.ws.jwtSubject.getValue();
    return currentJwt !== undefined && currentJwt !== '';
  }

  // Funktion til at vente et bestemt antal millisekunder
  delay(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  // Funktion der tjekker JWT med et retry loop
  async checkJwtWithRetry(): Promise<boolean> {
    for (let i = 0; i < 10; i++) {
      if (this.isLoggedIn()) {
        return true;
      }
      await this.delay(500); // Vent 1 sekund før næste tjek
    }
    return false;
  }
}
