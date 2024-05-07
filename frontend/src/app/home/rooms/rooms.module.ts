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
import {DeviceCardComponent} from "./room/device-card/device-card.component";
import {EditRoomComponent} from "./edit-room/edit-room.component";
import {RoomCardComponent} from "./room-card/room-card.component";


@NgModule({
  declarations: [RoomsComponent, RoomComponent, AllRoomsComponent, DeviceCardComponent, CreateRoomComponent, EditRoomComponent, RoomCardComponent],
  imports: [
    CommonModule,
    IonicModule,
    RoomsRoutingModule,
    HomePageModule,
    ReactiveFormsModule
  ],
  exports: [RoomCardComponent]
})
export class RoomsModule {
}
