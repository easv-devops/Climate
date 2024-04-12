import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRoutingModule } from './auth-routing.module';
import {BrowserModule} from "@angular/platform-browser";
import {IonicModule} from "@ionic/angular";
import {AppRoutingModule} from "../app-routing.module";
import {AuthComponent} from "./auth.component";
import {ReactiveFormsModule} from "@angular/forms";
import {LoginComponent} from "./login/login.component";
import {RegisterComponent} from "./register/register.component";


@NgModule({
  declarations: [AuthComponent, LoginComponent, RegisterComponent],
  imports: [
    CommonModule,
    IonicModule,
    AuthRoutingModule,
    ReactiveFormsModule
  ]
})
export class AuthModule { }
