import { Injectable } from '@angular/core';
import { ToastController } from '@ionic/angular';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {

  constructor(private toastController: ToastController) { }

  handleError(errorMessage: string) {
    //todo maybe log these at a later time
    this.presentToast(errorMessage);
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
