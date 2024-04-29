import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DevicesRoutingModule } from './devices-routing.module';
import {HomePageModule} from "../home.module";
import {DevicesComponent} from "./devices.component";
import {IonicModule} from "@ionic/angular";
import {DeviceCardComponent} from "./device-card/device-card.component";


@NgModule({
  declarations: [DevicesComponent, DeviceCardComponent],
  exports: [
    DeviceCardComponent
  ],
  imports: [
    CommonModule,
    DevicesRoutingModule,
    HomePageModule,
    IonicModule
  ]
})
export class DevicesModule { }
