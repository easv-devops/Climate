import {Component, Input, OnInit} from '@angular/core';
import {Device} from "../../../../models/device";

@Component({
  selector: 'app-device.ts-card',
  templateUrl: './device-card.component.html',
  styleUrls: ['./device-card.component.scss'],
})
export class DeviceCardComponent  implements OnInit {

  constructor() { }
  @Input() device!: Device;

  ngOnInit() {}

}
