import { Component, OnInit } from '@angular/core';
import {FormBuilder, Validators} from "@angular/forms";
import {ClientWantsToRegisterDto, ClientWantsToResetPassword} from "../../../models/clientRequests";
import {AuthService} from "../auth.service";
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../web-socket-connection.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-resetpassword',
  templateUrl: './resetpassword.component.html',
  styleUrls: ['./resetpassword.component.scss'],
})
export class ResetpasswordComponent  implements OnInit {

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
  });

  private unsubscribe$ = new Subject<void>();
  constructor(
    private readonly fb: FormBuilder,
    private authService: AuthService,
    public ws: WebSocketConnectionService,
    private router: Router
  ) {}

  ngOnInit() {
    this.ws.isReset.pipe(
      takeUntil(this.unsubscribe$)
    ).subscribe(isReset => {
      if (isReset) {
        //if password was successfully reset go to login.
        this.router.navigate(['auth/login']);
      }
    });
  }


  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  get email() {
    return this.form.controls.email;
  }

  resetPassword() {
    let user = new ClientWantsToResetPassword({
      Email: this.email.value!,
    })
    this.authService.resetPasswordWithEmail(user);
  }
}
