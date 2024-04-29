import {Component, OnInit, ViewChild} from '@angular/core';
import {Room} from "../../../../models/room";
import {OverlayEventDetail} from "@ionic/core/components";
import {IonModal} from "@ionic/angular";
import {RoomService} from "../room.service";
import {FormBuilder, Validators} from "@angular/forms";

@Component({
  selector: 'app-all-rooms',
  templateUrl: './all-rooms.component.html',
  styleUrls: ['../rooms.component.scss'],
})
export class AllRoomsComponent implements OnInit {
  rooms: Room[] = [
    {roomname:"Kitchen", roomId:3, currentHumidity: 20, currentTemperature: 23, image:"https://static.vecteezy.com/system/resources/thumbnails/006/689/881/small/kitchen-icon-illustration-free-vector.jpg"},
    {roomname: "Toilet", roomId:2, currentHumidity: 30, currentTemperature: 27, image: "https://cdn-icons-png.flaticon.com/512/194/194483.png"},
    {roomname: "Bedroom", roomId:1, currentHumidity: 15, currentTemperature: 25, image: "https://cdn-icons-png.flaticon.com/512/6192/6192020.png"},
    {roomname: "Livingroom", roomId:4, currentHumidity: 17, currentTemperature: 19, image: "https://cdn-icons-png.flaticon.com/512/2607/2607269.png"},
  ];
  roomname!: string;
  n: number = 4;

  form = this.fb.group({
    roomName: ['', [Validators.required, Validators.minLength(2)]]
  });
  constructor(private roomService: RoomService, private readonly fb: FormBuilder) {}

  ngOnInit() {}

  @ViewChild(IonModal) modal!: IonModal;
  cancel() {
    this.modal.dismiss(null, 'cancel');
  }

  confirm() {
    this.n++;
    this.rooms.push({roomname: this.roomname, roomId: this.n, image: "https://static.vecteezy.com/system/resources/thumbnails/006/689/880/small/bathroom-icon-illustration-free-vector.jpg", currentHumidity: 15, currentTemperature: 23})
    this.roomService.createRoom(this.roomname);
    this.modal.dismiss(this.roomname, 'confirm');
  }
  onWillDismiss(event: Event) {
    const ev = event as CustomEvent<OverlayEventDetail<string>>;
    if (ev.detail.role === 'confirm') {

    }
  }

  get roomName() {
    return this.form.controls.roomName;
  }
}
