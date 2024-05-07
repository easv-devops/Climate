import { Component, OnInit } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";

@Component({
  selector: 'app-edit-room',
  templateUrl: './edit-room.component.html',
  styleUrls: ['./edit-room.component.scss'],
})
export class EditRoomComponent  implements OnInit {

  readonly form = this.fb.group({
    roomName: ['', Validators.required]
  });

  get roomName() {
    return this.form.controls.roomName;
  }

  constructor(private readonly fb: FormBuilder) { }

  ngOnInit() {}

  editRoom() {

  }
}
