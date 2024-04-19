import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {CreateDeviceComponent} from "./create-device/create-device.component";

const routes: Routes = [
  {
    path: '',
    component: CreateDeviceComponent,
    children: [
      {
        path: 'device',
        component: CreateDeviceComponent
      }
      ]
  }
  ];

    @NgModule({
      imports: [RouterModule.forChild(routes)],
      exports: [RouterModule]
    })

    export class DeviceRoutingModule {}
