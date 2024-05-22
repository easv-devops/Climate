import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {IonicModule} from '@ionic/angular';
import {FormsModule} from '@angular/forms';
import {HomePage} from './home.page';

import {HomePageRoutingModule} from './home-routing.module';
import {TopbarComponent} from "./topbar/topbar.component";
import {GraphComponent} from "./graphs/graph/graph.component";
import {NgApexchartsModule} from "ng-apexcharts";
import {LandingPageComponent} from "./landing-page/landing-page.component";


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    HomePageRoutingModule,
    NgApexchartsModule
  ],
  exports: [
    GraphComponent,
    TopbarComponent,
  ],
  declarations: [HomePage, TopbarComponent, GraphComponent]
})
export class HomePageModule {
}
