import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {IonicModule} from '@ionic/angular';
import {FormsModule} from '@angular/forms';
import {HomePage} from './home.page';

import {HomePageRoutingModule} from './home-routing.module';
import {TopbarComponent} from "./topbar/topbar.component";
import {GraphComponent} from "./graph/graph.component";


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    HomePageRoutingModule
  ],
  exports: [
    GraphComponent,
    TopbarComponent
  ],
  declarations: [HomePage, TopbarComponent, GraphComponent]
})
export class HomePageModule {}
