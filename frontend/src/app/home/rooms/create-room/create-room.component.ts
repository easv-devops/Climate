import { Component, OnInit } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {RoomService} from "../room/room.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-create-room',
  templateUrl: './create-room.component.html',
  styleUrls: ['./create-room.component.scss'],
})
export class CreateRoomComponent  implements OnInit {

  readonly form = this.fb.group({
    roomName: ['', Validators.required]
  });

  constructor(private readonly fb: FormBuilder, private readonly roomService: RoomService,
              private readonly router: Router) { }

  ngOnInit() {}


  get roomName() {
    return this.form.controls.roomName;
  }

  createRoom() {
    this.roomService.createRoom(this.roomName.value!)
    this.router.navigate(["rooms/all"])
  }


}
