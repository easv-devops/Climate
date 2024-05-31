import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {HomePage} from './home.page';


import {LandingPageComponent} from "./landing-page/landing-page.component";
import {AllRoomsComponent} from "./rooms/all-rooms/all-rooms.component";
import {AlertComponent} from "./alert/alert.component";
import {AuthGuard} from "../guards/AuthGuard";

const routes: Routes = [
  {
    path: '',
    component: HomePage,
    children: [ // Child routes for authentication
      {
        path: '',
        redirectTo: 'rooms/all',
        pathMatch: "full"
      },
      {
        path: 'landing', //Path for landing zone
        component: LandingPageComponent
      },
      {
        path: 'alerts', //Path for alert
        component: AlertComponent
      },
      {
        path: 'rooms', //Path for rooms module, which loads own children (rooms-routing.module)
        loadChildren: () => import('./rooms/rooms.module').then(m => m.RoomsModule)
      },
      {
        path: 'devices', //Path for devices module, which loads own children (devices-routing.module)
        loadChildren: () => import('./devices/devices.module').then(m => m.DevicesModule)
      },
      {
        path: 'user', //Path for devices module, which loads own children (devices-routing.module)
        loadChildren: () => import('./user/user.module').then(m => m.UserModule)
      }
    ],
    canActivate: [AuthGuard]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomePageRoutingModule {
}
