import { Component, OnInit } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {RoomService} from "../room/room.service";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-edit-room',
  templateUrl: './edit-room.component.html',
  styleUrls: ['./edit-room.component.scss'],
})
export class EditRoomComponent  implements OnInit {

  readonly form = this.fb.group({
    roomName: ['', Validators.required]
  });
  private idFromRoute?: number;

  get roomName() {
    return this.form.controls.roomName;
  }

  constructor(private readonly fb: FormBuilder,
              private readonly roomService: RoomService,
              private activatedRoute: ActivatedRoute,
              private readonly router: Router) { }

  ngOnInit() {
    this.getRoomFromRoute()
  }

  editRoom() {
    this.roomService.editRoom(this.idFromRoute!, this.roomName.value!);
    this.router.navigate(["rooms/"+this.idFromRoute]);
  }

  getRoomFromRoute() {
    this.idFromRoute = +this.activatedRoute.snapshot.params['id'];
  }
}
