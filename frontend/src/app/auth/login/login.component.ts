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

send() {
    this.ws.socketConnection.send('asdasd')
}

  get email() {
    return this.form.controls['email'];
  }

  get password() {
    return this.form.controls['password'];
  }


  //todo should call a auth service for connect to api
  submit() {
    this.authService.loginUser();
  }
}
