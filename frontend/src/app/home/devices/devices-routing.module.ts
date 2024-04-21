import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {DevicesComponent} from "./devices.component";
import {DeviceComponent} from "./device/device.component";

const routes: Routes = [
  {
    path: '',
    component: DevicesComponent,
    children: [
      {
        path: ':id',
        component: DeviceComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DevicesRoutingModule { }
