
import { Component } from '@angular/core';
// @ts-ignore
import { BluetoothCore } from '@abandonware/web-bluetooth';

@Component({
  selector: 'app-webbluetooth',
  templateUrl: './webbluetooth.component.html',
  styleUrls: ['./webbluetooth.component.scss']
})
export class WebBluetoothComponent {

  constructor(private readonly ble: BluetoothCore) { }

  async search() {
    try {
      const device = await this.ble.requestDevice({
        acceptAllDevices: true
      });
      console.log('Device found:', device);
    } catch (error) {
      console.error('Error:', error);
    }
  }

}
