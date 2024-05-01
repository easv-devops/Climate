// webbluetooth.component.ts
import { Component } from '@angular/core';
import {bluetooth} from "webbluetooth";


// Import typings for Web Bluetooth API
//import './typings/web-bluetooth';
import {from} from "rxjs";

@Component({
  selector: 'app-webbluetooth',
  templateUrl: './webbluetooth.component.html',
  styleUrls: ['./webbluetooth.component.scss']
})
export class WebBluetoothComponent {
  scannedDevices: BluetoothDevice[] = [];


  conuctor() {  }

  async connectToDevice(device: BluetoothDevice) {
    try {
      // Check if device.gatt is defined before accessing it
      if (!device.gatt) {
        console.error('Device GATT is not available.');
        return;
      }

      const server = await device.gatt.connect();
      const services = await server.getPrimaryServices();

      // Iterate through services
      for (const service of services) {
        console.log('Service:', service.uuid);
        const characteristics = await service.getCharacteristics();

        // Iterate through characteristics
        for (const characteristic of characteristics) {
          console.log('Characteristic:', characteristic.uuid);
          // Read/write to characteristics as needed
        }
      }
    } catch (error) {
      console.error('Error:', error);
    }
  }


  async scanForDevices() {
    try {
      // Request any Bluetooth device
      const device = await navigator.bluetooth.requestDevice({
        acceptAllDevices: true
      });

      // Add the scanned device to the list
      this.scannedDevices.push(device);
    } catch (error) {
      console.error('Error:', error);
    }
  }

}
