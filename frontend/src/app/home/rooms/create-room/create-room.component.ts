import { Component, OnInit } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";

@Component({
  selector: 'app-create-room',
  templateUrl: './create-room.component.html',
  styleUrls: ['./create-room.component.scss'],
})
export class CreateRoomComponent  implements OnInit {

  readonly form = this.fb.group({
    roomName: ['', Validators.required]
  });

  constructor(private readonly fb: FormBuilder) { }

  ngOnInit() {}


  get roomName() {
    return this.form.controls.roomName;
  }

  createRoom() {
  }
}
