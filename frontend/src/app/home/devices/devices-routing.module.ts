import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {DevicesComponent} from "./devices.component";
import {DeviceComponent} from "./device/device.component";
import {CreateDeviceComponent} from "./create-device/create-device.component";
import {EditDeviceComponent} from "./edit-device/edit-device.component";

const routes: Routes = [
  {
    path: '',
    component: DevicesComponent,
    children: [
      {
        path: 'add',
        component: CreateDeviceComponent
      },
      {
        path: ':id',
        component: DeviceComponent
      },
      {
        path: ':id/edit',
        component: EditDeviceComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DevicesRoutingModule { }
