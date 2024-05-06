import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {RoomsComponent} from "./rooms.component";
import {AllRoomsComponent} from "./all-rooms/all-rooms.component";
import {RoomComponent} from "./room/room.component";
import {EditRoomComponent} from "./edit-room/edit-room.component";
import {CreateRoomComponent} from "./create-room/create-room.component";

const routes: Routes = [
  {
    path: '',
    component: RoomsComponent,
    children: [ // Child routes for rooms
      {
        path: 'all',
        component: AllRoomsComponent
      },
      {
        path: ':id',
        component: RoomComponent
      },
      {
        path: ':id/edit',
        component: EditRoomComponent
      },
      {
        path: 'create',
        component: CreateRoomComponent
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RoomsRoutingModule { }
