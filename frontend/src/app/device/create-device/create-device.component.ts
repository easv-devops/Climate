import {Component, OnInit} from '@angular/core';
import {FormBuilder, Validators} from '@angular/forms';
import { IonicModule } from '@ionic/angular';

@Component({
  selector: 'app-create-device',
  templateUrl: './create-device.component.html',
  styleUrls: ['./create-device.component.scss'],
})

export class CreateDeviceComponent implements OnInit {

  readonly form = this.fb.group({
    deviceName: ['', Validators.required],
  });

  constructor(
    private readonly fb: FormBuilder
  ) {}

  ngOnInit() {}

  get deviceName() {
    return this.form.controls.deviceName;
  }

  async createDevice() {

  }
}
