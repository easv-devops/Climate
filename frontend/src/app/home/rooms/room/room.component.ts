import {Component, Input, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Room} from "../../../../models/room";
import {Device} from "../../../../models/device";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./../rooms.component.scss'],
})
export class RoomComponent  implements OnInit {
  roomId: number | undefined;
  devices: Device[] = [{deviceId: 1},{deviceId: 2},{deviceId: 3}];
  deviceName?: string;

  @Input() room!: Room;

  constructor(private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.roomId = this.activatedRoute.snapshot.params['id'];
    console.log(this.room.roomname)
  }


  onWillDismiss($event: any) {
  }
  confirm(){
  }
  cancel() {

  }
}
