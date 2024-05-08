import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {DevicesRoutingModule} from './devices-routing.module';
import {HomePageModule} from "../home.module";
import {DevicesComponent} from "./devices.component";
import {IonicModule} from "@ionic/angular";
import {DeviceComponent} from "./device/device.component";
import {CreateDeviceComponent} from "./create-device/create-device.component";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {EditDeviceComponent} from "./edit-device/edit-device.component";


@NgModule({
  declarations: [DevicesComponent, DeviceComponent, CreateDeviceComponent, EditDeviceComponent],
  imports: [
    CommonModule,
    DevicesRoutingModule,
    HomePageModule,
    IonicModule,
    ReactiveFormsModule,
    FormsModule
  ]
})
export class DevicesModule { }
