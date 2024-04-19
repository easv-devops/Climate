import { Component } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {AuthService} from "../auth.service";
import {WebSocketConnectionService} from "../../web-socket-connection.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {

  readonly form : FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(7)]],
  });

  constructor(private readonly fb: FormBuilder, private authService: AuthService, public ws: WebSocketConnectionService){
  }

  get email() {
    return this.form.controls['email'];
  }

  get password() {
    return this.form.controls['password'];
  }

  submitTestUser(){
    this.authService.loginUser("user@example.com", "12345678");
  }

  submit() {
    if (this.form.get('email') && this.form.get('password')) {
      //The ?? operator works like this:
      //const value = possiblyNullOrUndefinedValue ?? defaultValue;
      const email: string = this.form.get('email')?.value ?? '';
      const password: string = this.form.get('password')?.value ?? '';
      this.authService.loginUser(email, password);
    }
    //Handles if an error occurs
    else {
      console.error('Was not able to get the required information');
    }
  }
}
