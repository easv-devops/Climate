import { Component, OnInit } from '@angular/core';
import {ClientWantsToDeleteDevice} from "../../../../models/clientRequests";
import {DeviceService} from "../../devices/device.service";

@Component({
  selector: 'app-device-settings',
  templateUrl: './device-settings.component.html',
  styleUrls: ['./device-settings.component.scss'],
})
export class DeviceSettingsComponent  implements OnInit {
  idFromRoute: any;

  constructor(public deviceService: DeviceService) { }

  ngOnInit() {}

  public deviceId: number | undefined;
  deleteDevice() {

  }
}
