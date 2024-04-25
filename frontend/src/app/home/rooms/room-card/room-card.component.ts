import {Component, Input, OnInit} from '@angular/core';
import {Room} from "../../../../models/room";

@Component({
  selector: 'app-room-card',
  templateUrl: './room-card.component.html',
  styleUrls: ['../rooms.component.scss'],
})
export class RoomCardComponent  implements OnInit {

  constructor() {
  }
  @Input() roomname!: Room;

  ngOnInit() {
  }
}
