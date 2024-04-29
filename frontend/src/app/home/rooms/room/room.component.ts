import {Component, Input, OnInit} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
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
  roomName?: string;

  public alertButtons = [
    {
      text: 'Cancel',
      role: 'cancel',
      handler: () => {
        console.log('Alert canceled');
      },
    },
    {
      text: 'OK',
      role: 'confirm',
      handler: () => {
        console.log('Alert confirmed');
      },
    },
  ];

  constructor(private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.roomId = this.activatedRoute.snapshot.params['id'];
  }


  onWillDismiss($event: any) {
  }
  confirm(){
  }
  cancel() {

  }
}
