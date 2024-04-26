import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {DevicesRoutingModule} from './devices-routing.module';
import {HomePageModule} from "../home.module";
import {DevicesComponent} from "./devices.component";
import {IonicModule} from "@ionic/angular";
import {DeviceComponent} from "./device/device.component";


@NgModule({
  declarations: [DevicesComponent, DeviceComponent],
  imports: [
    CommonModule,
    DevicesRoutingModule,
    HomePageModule,
    IonicModule
  ]
})
export class DevicesModule { }
