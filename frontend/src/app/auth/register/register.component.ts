import {Component, OnInit} from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {AuthService} from "../auth.service";
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {ClientWantsToRegisterDto} from "../../../models/clientRequests";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent  implements OnInit {

  readonly form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    repeatPassword: ['', [Validators.required]],
    phone: ['', Validators.required],
  });

  isPasswordSame = false;

  private unsubscribe$ = new Subject<void>();

  constructor(
    private readonly fb: FormBuilder,
    private authService: AuthService,
    public ws: WebSocketConnectionService,
    private router: Router
  ) {
  }

  ngOnInit() {
    // Subscribe to changes in password and repeatPassword fields
    this.form.get('password')!.valueChanges.subscribe(() => this.checkPasswords());
    this.form.get('repeatPassword')!.valueChanges.subscribe(() => this.checkPasswords());

    // Subscribe to jwt observable
    this.ws.jwt.pipe(
      takeUntil(this.unsubscribe$)
    ).subscribe(jwt => {
      if (jwt) {
        // JWT is received, performs redirection
        this.router.navigate(['']);
      }
    });
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  get firstName() {
    return this.form.controls.firstName;
  }

  get lastName() {
    return this.form.controls.lastName;
  }

  get email() {
    return this.form.controls.email;
  }

  get phone() {
    return this.form.controls.phone;
  }

  get password() {
    return this.form.controls.password;
  }

  get repeatPassword() {
    return this.form.controls.repeatPassword;
  }

  register() {
      let user = new ClientWantsToRegisterDto({
        Email: this.email.value!,
        CountryCode: "+45",//todo get it from the form control when country code is added
        Phone: this.phone.value!,
        FirstName: this.firstName.value!,
        LastName: this.lastName.value!,
        Password: this.password.value!
      })
      this.authService.registerUser(user);
    }

  checkPasswords() {
    const password = this.form.get('password')!.value;
    const repeatPassword = this.form.get('repeatPassword')!.value;
    this.isPasswordSame = password === repeatPassword;
  }

}
