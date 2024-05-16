import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserRoutingModule } from './user-routing.module';
import {ReactiveFormsModule} from "@angular/forms";
import {EditUserComponent} from "./edit-user/edit-user.component";
import {IonicModule} from "@ionic/angular";


@NgModule({
  declarations: [EditUserComponent],
  imports: [
    CommonModule,
    IonicModule,
    UserRoutingModule,
    ReactiveFormsModule
  ]
})
export class UserModule { }
