import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';

import { DeviceRoutingModule } from './device-routing.module';
import {CreateDeviceComponent} from "./create-device/create-device.component";
import {ReactiveFormsModule} from "@angular/forms";


@NgModule({
  declarations: [CreateDeviceComponent],
  imports: [
    CommonModule,
    DeviceRoutingModule,
    IonicModule,
    ReactiveFormsModule,

  ]
})
export class DeviceModule { }
