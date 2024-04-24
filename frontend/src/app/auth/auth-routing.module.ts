import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from "./login/login.component";
import {RegisterComponent} from "./register/register.component";
import {AuthComponent} from "./auth.component";
import {ResetpasswordComponent} from "./resetpassword/resetpassword.component";

const routes: Routes = [
  {
    path: '', // Base path for authentication
    component: AuthComponent, // This will be loaded into the nested router outlet
    children: [ // Child routes for authentication
      {
        path: 'login', // Path for login component (e.g., /auth/login)
        component: LoginComponent
      },
      {
        path: 'register', // Path for register component (e.g., /auth/register)
        component: RegisterComponent
      },
      {
        path: 'resetpassword',
        component: ResetpasswordComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class AuthRoutingModule { }
