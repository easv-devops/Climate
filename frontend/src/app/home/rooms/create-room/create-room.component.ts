import { Component, OnInit } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {RoomService} from "../room.service";
import {ActivatedRoute, Router} from "@angular/router";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";

@Component({
  selector: 'app-create-room',
  templateUrl: './create-room.component.html',
})
export class CreateRoomComponent  implements OnInit {
  readonly form = this.fb.group({
    roomName: ['', Validators.required],
  });


  constructor(private readonly fb: FormBuilder,
              private readonly roomService : RoomService,
              public ws: WebSocketConnectionService,
              private readonly router: Router) { }

  ngOnInit() {
    console.log("inside create")
  }

  get roomName() {
    return this.form.controls.roomName;
  }

  createRoom() {
    this.roomService.createRoom(this.roomName.value!);
    this.router.navigate(['/rooms/all']);
  }
}
