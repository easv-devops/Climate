import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.scss'],
})
export class RoomComponent  implements OnInit {
  roomId: number | undefined;

  constructor(private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.roomId = this.activatedRoute.snapshot.params['id'];
  }

}
