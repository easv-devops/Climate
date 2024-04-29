import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomePage } from './home.page';

import {Page1Component} from "./page1/page1.component";
import {Page2Component} from "./page2/page2.component";

const routes: Routes = [
  {
    path: '',
    component: HomePage,
    children: [ // Child routes for authentication
      {
        path: 'page1', // Path for login component (e.g., /auth/login)
        component: Page1Component
      },
      {
        path: 'page2', // Path for login component (e.g., /auth/login)
        component: Page2Component
      },
      {
        path: 'rooms', //Path for rooms module, which loads own children (rooms-routing.module)
        loadChildren: () => import('./rooms/rooms.module').then(m => m.RoomsModule)
      },
      {
        path: 'devices', //Path for devices module, which loads own children (devices-routing.module)
        loadChildren: () => import('./devices/devices.module').then(m => m.DevicesModule)
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomePageRoutingModule {}
