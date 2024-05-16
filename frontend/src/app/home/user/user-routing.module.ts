import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {EditUserComponent} from "./edit-user/edit-user.component";

const routes: Routes = [
  {
    path: '',
    component: EditUserComponent,
    /**
     * As discussed we only want an edit-user component in this module for now.
     * So /user loads EditUserComponent, but if we want more children this is how:
    children: [
      {
        path: 'edit',
        component: EditUserComponent
      },
    ]**/
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRoutingModule { }
