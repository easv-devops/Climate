import { Component, OnInit } from '@angular/core';
import {ToastController} from "@ionic/angular";
import {WebSocketConnectionService} from "../web-socket-connection.service";

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss'],
})
export class AuthComponent  implements OnInit {

  constructor(private toastController: ToastController) { }

  ngOnInit() {

  }

  async presentToast(errorMessage: string) {
    const toast = await this.toastController.create({
      message: errorMessage,
      duration: 4000,
      position: 'bottom',
      color: 'danger'
    });
    toast.present();
  }

}
