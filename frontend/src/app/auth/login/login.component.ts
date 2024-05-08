import {Component} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../auth.service";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {Subject, takeUntil} from "rxjs";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {

  readonly form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(7)]],
  });

  private unsubscribe$ = new Subject<void>();

  constructor(private readonly fb: FormBuilder, private authService: AuthService, public ws: WebSocketConnectionService, private router: Router) {
  }
  ngOnInit() {
    // Subscribe to jwt observable
    this.ws.jwt.pipe(
      takeUntil(this.unsubscribe$)
    ).subscribe(jwt => {
      if (jwt && jwt != '') {
        // JWT is received, perform redirection or other actions here
        this.router.navigate(['']);
      }
    });
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  get email() {
    return this.form.controls['email'];
  }

  get password() {
    return this.form.controls['password'];
  }

  submitTestUser() {
    this.authService.loginUser("user@mail.com", "12345678");
  }

  submit() {
    if (this.form.get('email') && this.form.get('password')) {
      //The ?? operator works like this:
      //const value = possiblyNullOrUndefinedValue ?? defaultValue;
      //todo should be from form group instead (so it is validated) look in register component for example code
      const email: string = this.form.get('email')!.value ?? '';
      const password: string = this.form.get('password')!.value ?? '';
      this.authService.loginUser(email, password);
    }
    //Handles if an error occurs
    else {
      console.error('Was not able to get the required information');
    }
  }

  RedirectToForgotPassword() {
    this.router.navigate(['auth/resetpassword']);
  }
}
