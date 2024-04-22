import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-device',
  templateUrl: './device.component.html',
  styleUrls: ['./device.component.scss'],
})
export class DeviceComponent  implements OnInit {
  deviceId: number | undefined;

  constructor(private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.deviceId = this.activatedRoute.snapshot.params['id'];
  }

}
