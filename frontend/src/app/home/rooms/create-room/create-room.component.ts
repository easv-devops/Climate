import {Component} from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {RoomService} from "../room.service";
import {Router} from "@angular/router";
import {Subject} from "rxjs";

@Component({
  selector: 'app-create-room',
  templateUrl: './create-room.component.html',
  styleUrls: ['./create-room.component.scss'],
})
export class CreateRoomComponent {

  readonly form = this.fb.group({
    roomName: ['', Validators.required]
  });

  private unsubscribe$ = new Subject<void>();

  constructor(private readonly fb: FormBuilder,
              private readonly roomService: RoomService,
              private readonly router: Router) {}

  get roomName() {
    return this.form.controls.roomName;
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  createRoom() {
    this.roomService.createRoom(this.roomName.value!)
    this.router.navigate(["rooms/all"]);
  }
}
