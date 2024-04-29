import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {DevicesRoutingModule} from './devices-routing.module';
import {HomePageModule} from "../home.module";
import {DevicesComponent} from "./devices.component";
import {IonicModule} from "@ionic/angular";
import {DeviceComponent} from "./device/device.component";
import {CreateDeviceComponent} from "./create-device/create-device.component";
import {ReactiveFormsModule} from "@angular/forms";


@NgModule({
  declarations: [DevicesComponent, DeviceComponent, CreateDeviceComponent],
  imports: [
    CommonModule,
    DevicesRoutingModule,
    HomePageModule,
    IonicModule,
    ReactiveFormsModule
  ]
})
export class DevicesModule { }
