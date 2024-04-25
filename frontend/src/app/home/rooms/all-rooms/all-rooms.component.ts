import { Component, OnInit } from '@angular/core';
import {Room} from "../../../../models/room";

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
  ];
  constructor() { }

  ngOnInit() {}

}
