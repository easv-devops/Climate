import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-room-card',
  template: `
    <ion-card>
      <ion-card-header>
        Device ID: {{ deviceId?.toString() }}
      </ion-card-header>
      <!-- Add more content here as needed -->
    </ion-card>
  `,
})
export class RoomCardComponent {
  @Input() deviceId: number | undefined;

  constructor() { }
}
