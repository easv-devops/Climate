import {Injectable} from '@angular/core';
import {CanActivate, Router} from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor( private router: Router) {}

  async canActivate() {
    // Tjek om brugeren har en gyldig JWT
    if (this.isLoggedIn()) {
      return true; // Tillad adgang til ruten, hvis brugeren er logget ind

    } else {

      // Hvis brugeren ikke er logget ind, omdiriger til login-siden
      this.router.navigate(['/auth/login']);
      return false; // Bloker adgang til ruten
    }
  }

  isLoggedIn(): boolean {
    const currentJwt = localStorage.getItem('jwt');
    return currentJwt !== undefined && currentJwt !== '';
  }
}
