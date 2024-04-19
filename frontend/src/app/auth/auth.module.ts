import {AuthService} from "./auth.service";
import {NgModule} from "@angular/core";
import {AuthComponent} from "./auth.component";
import {LoginComponent} from "./login/login.component";
import {RegisterComponent} from "./register/register.component";
import {ResetpasswordComponent} from "./resetpassword/resetpassword.component";
import {CommonModule, NgOptimizedImage} from "@angular/common";
import {IonicModule} from "@ionic/angular";
import {AuthRoutingModule} from "./auth-routing.module";
import {ReactiveFormsModule} from "@angular/forms";


@NgModule({
  providers:[
    AuthService
  ],
  declarations: [
    AuthComponent,
    LoginComponent,
    RegisterComponent,
    ResetpasswordComponent
  ],
  imports: [
    CommonModule,
    IonicModule,
    AuthRoutingModule,
    ReactiveFormsModule,
    NgOptimizedImage
  ]
})
export class AuthModule {
}
