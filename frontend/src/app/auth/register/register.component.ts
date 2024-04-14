import { Component, OnInit } from '@angular/core';
import {AbstractControl, FormBuilder, ValidationErrors, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {ToastController} from "@ionic/angular";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent  implements OnInit {

  readonly form = this.fb.group({
    name: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    repeatPassword: ['', [Validators.required]],
    phone: ['', Validators.required],
  });

  isPasswordSame = false;
  constructor(
    private readonly fb: FormBuilder,
  ) {
  }

  ngOnInit() {
    // Subscribe to changes in password and repeatPassword fields
    this.form.get('password')!.valueChanges.subscribe(() => this.checkPasswords());
    this.form.get('repeatPassword')!.valueChanges.subscribe(() => this.checkPasswords());
  }

  get name() {
    return this.form.controls.name;
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

  }


  checkPasswords() {
    const password = this.form.get('password')!.value;
    const repeatPassword = this.form.get('repeatPassword')!.value;
    this.isPasswordSame = password === repeatPassword;
  }

}
