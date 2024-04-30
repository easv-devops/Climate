import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor( private router: Router) {}

  canActivate(): boolean {
    // Tjek om brugeren har en gyldig JWT
    if (this.isLoggedIn()) {
      return true; // Tillad adgang til ruten, hvis brugeren er logget ind
    } else {
      // Hvis brugeren ikke er logget ind, omdiriger til login-siden
      this.router.navigate(['/auth/login']);
      return false; // Bloker adgang til ruten
    }
  }

  //todo temporary logged in method (should be some more)
  isLoggedIn(): boolean {
    // Tjek om JWT findes i localStorage eller om den er udløbet
    const jwt = localStorage.getItem('jwt');
    return !!jwt; // Returner true, hvis JWT findes
  }
}
