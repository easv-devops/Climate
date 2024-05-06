import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {RoomsRoutingModule} from './rooms-routing.module';
import {IonicModule} from "@ionic/angular";
import {RoomsComponent} from "./rooms.component";
import {RoomComponent} from "./room/room.component";
import {AllRoomsComponent} from "./all-rooms/all-rooms.component";
import {HomePageModule} from "../home.module";
import {RoomCardComponent} from "./room-card/room-card.component";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {AuthService} from "../../auth/auth.service";
import {RoomService} from "./room.service";
import {DevicesModule} from "../devices/devices.module";
import {EditRoomComponent} from "./edit-room/edit-room.component";
import {CreateRoomComponent} from "./create-room/create-room.component";


@NgModule({
  providers:[
    RoomService
  ],
  declarations: [RoomsComponent, RoomComponent, AllRoomsComponent, RoomCardComponent, EditRoomComponent, CreateRoomComponent],
    imports: [
        CommonModule,
        IonicModule,
        RoomsRoutingModule,
        HomePageModule,
        FormsModule,
        ReactiveFormsModule
    ]
})
export class RoomsModule {
}
