import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {RouteReuseStrategy} from '@angular/router';

import {IonicModule, IonicRouteStrategy} from '@ionic/angular';

import {AppComponent} from './app.component';
import {AppRoutingModule} from './app-routing.module';
import {HomePageModule} from "./home/home.module";
import {AuthGuard} from "./guards/AuthGuard";
import {AgGridModule} from "ag-grid-angular";

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, IonicModule.forRoot(), HomePageModule, AppRoutingModule, AgGridModule],
  providers: [{provide: RouteReuseStrategy, useClass: IonicRouteStrategy}, AuthGuard],
  bootstrap: [AppComponent],
})
export class AppModule {

}
