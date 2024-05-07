import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {RoomsComponent} from "./rooms.component";
import {AllRoomsComponent} from "./all-rooms/all-rooms.component";
import {RoomComponent} from "./room/room.component";
import {CreateRoomComponent} from "./create-room/create-room.component";
import {EditRoomComponent} from "./edit-room/edit-room.component";

const routes: Routes = [
  {
    path: '',
    component: RoomsComponent,
    children: [ // Child routes for authentication
      {
        path: 'add', // Path for creating a room
        component: CreateRoomComponent
      },
      {
        path: 'edit', // Path for creating a room
        component: EditRoomComponent
      },
      {
        path: 'all', // Path for login component (e.g., /auth/login)
        component: AllRoomsComponent
      },
      {
        path: ':id', // Path for login component (e.g., /auth/login)
        component: RoomComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RoomsRoutingModule { }
