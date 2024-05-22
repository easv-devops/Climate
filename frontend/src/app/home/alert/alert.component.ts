interface Alertss {
  timestamp: string;
  isRead: boolean;
  description: string;
  deviceName: string;
  roomName: string;
}

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss']
})



export class AlertComponent implements OnInit {

  sortedAlerts: Alertss[] = [];
  sortByOrder: { [key: string]: string } = {};

  alerts = [
    { timestamp: '2024-05-22 10:00:00', isRead: false, description: 'Temperature threshold exceeded', deviceName: 'Temp Sensor 1', roomName: 'Living Room' },
    { timestamp: '2024-05-22 10:15:00', isRead: true, description: 'Low battery', deviceName: 'Smoke Detector', roomName: 'Bedroom' },
    { timestamp: '2024-05-22 10:30:00', isRead: false, description: 'Device offline', deviceName: 'Motion Sensor', roomName: 'Kitchen' },
    { timestamp: '2024-05-22 10:45:00', isRead: false, description: 'Door left open', deviceName: 'Door Sensor', roomName: 'Entryway' },
    { timestamp: '2024-05-22 11:00:00', isRead: true, description: 'Humidity threshold exceeded', deviceName: 'Humidity Sensor', roomName: 'Bathroom' },
    { timestamp: '2024-05-22 11:15:00', isRead: false, description: 'Water leak detected', deviceName: 'Water Leak Sensor', roomName: 'Basement' },
    { timestamp: '2024-05-22 11:30:00', isRead: true, description: 'Motion detected', deviceName: 'Motion Sensor 2', roomName: 'Living Room' },
    { timestamp: '2024-05-22 11:45:00', isRead: false, description: 'CO2 level above threshold', deviceName: 'CO2 Sensor', roomName: 'Office' },
    { timestamp: '2024-05-22 12:00:00', isRead: false, description: 'Window open', deviceName: 'Window Sensor', roomName: 'Bedroom' },
    { timestamp: '2024-05-22 12:15:00', isRead: true, description: 'Fire alarm detected', deviceName: 'Fire Alarm', roomName: 'Living Room' }
  ]; //todo hente rigtige alerts fra devices og slet hardcoded



  constructor() {}

  ngOnInit() {this.sortedAlerts = [...this.alerts];}

  sortBy(column: keyof Alertss) {
    console.log('Sorting by', column);

    // Skifter mellem sorteringsrækkefølgen stigende og faldende
    this.sortByOrder[column] = !this.sortByOrder[column] || this.sortByOrder[column] === 'asc' ? 'desc' : 'asc';

    // Nulstiller sorteringsrækkefølgen for andre kolonner
    Object.keys(this.sortByOrder).forEach(key => {
      if (key !== column) {
        this.sortByOrder[key] = '';
      }
    });

    // Sorter array af alerts ud fra valgt kolonne og sorteringsrækkefølgen
    this.sortedAlerts.sort((a, b) => {
      if (a[column] < b[column]) return this.sortByOrder[column] === 'asc' ? -1 : 1;
      if (a[column] > b[column]) return this.sortByOrder[column] === 'asc' ? 1 : -1;
      return 0;
    });
  }
}
