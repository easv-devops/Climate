import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {RoomsRoutingModule} from './rooms-routing.module';
import {IonicModule} from "@ionic/angular";
import {RoomsComponent} from "./rooms.component";
import {RoomComponent} from "./room/room.component";
import {AllRoomsComponent} from "./all-rooms/all-rooms.component";
import {HomePageModule} from "../home.module";
import {CreateRoomComponent} from "./create-room/create-room.component";
import {ReactiveFormsModule} from "@angular/forms";
import {RoomCardComponent} from "./room/device-card/device-card.component";
import {EditRoomComponent} from "./edit-room/edit-room.component";


@NgModule({
  declarations: [RoomsComponent, RoomComponent, AllRoomsComponent, RoomCardComponent, CreateRoomComponent, EditRoomComponent],
  imports: [
    CommonModule,
    IonicModule,
    RoomsRoutingModule,
    HomePageModule,
    ReactiveFormsModule
  ]
})
export class RoomsModule {
}
