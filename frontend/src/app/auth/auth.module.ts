import { NgModule } from '@angular/core';
import {CommonModule, NgOptimizedImage} from '@angular/common';

import { AuthRoutingModule } from './auth-routing.module';
import {BrowserModule} from "@angular/platform-browser";
import {IonicModule} from "@ionic/angular";
import {AppRoutingModule} from "../app-routing.module";
import {AuthComponent} from "./auth.component";
import {ReactiveFormsModule} from "@angular/forms";
import {LoginComponent} from "./login/login.component";
import {RegisterComponent} from "./register/register.component";
import {ResetpasswordComponent} from "./resetpassword/resetpassword.component";


@NgModule({
  declarations: [AuthComponent, LoginComponent, RegisterComponent, ResetpasswordComponent],
  imports: [
    CommonModule,
    IonicModule,
    AuthRoutingModule,
    ReactiveFormsModule,
    NgOptimizedImage
  ]
})
export class AuthModule { }
