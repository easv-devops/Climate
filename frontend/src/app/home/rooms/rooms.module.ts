import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {RoomsRoutingModule} from './rooms-routing.module';
import {IonicModule} from "@ionic/angular";
import {RoomsComponent} from "./rooms.component";
import {RoomComponent} from "./room/room.component";
import {AllRoomsComponent} from "./all-rooms/all-rooms.component";
import {HomePageModule} from "../home.module";
import {RoomCardComponent} from "./room-card/room-card.component";


@NgModule({
  declarations: [RoomsComponent, RoomComponent, AllRoomsComponent, RoomCardComponent],
  imports: [
    CommonModule,
    IonicModule,
    RoomsRoutingModule,
    HomePageModule
  ]
})
export class RoomsModule {
}
