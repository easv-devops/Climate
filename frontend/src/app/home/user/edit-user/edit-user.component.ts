import {Component, OnInit} from '@angular/core';
import {Subject, takeUntil} from "rxjs";
import {WebSocketConnectionService} from "../../../web-socket-connection.service";
import {Router} from "@angular/router";
import {FullUserDto} from "../../../../models/ServerSendsUser";
import {FormBuilder, Validators} from "@angular/forms";
import {ClientWantsToEditUserInfoDto} from "../../../../models/ClientWantsToEditUserInfoDto";
import {UserService} from "../user.service";
import {CountryCode} from "../../../../models/Entities";
import {ClientWantsToGetCountryCodeDto} from "../../../../models/ClientWantsToGetCountryCode";

@Component({
  selector: 'app-edit-user',
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.scss'],
})
export class EditUserComponent  implements OnInit {
  private unsubscribe$ = new Subject<void>();
  user: FullUserDto | undefined;
  public allCountryCodes: CountryCode[] | undefined;

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    countryCode: ['', Validators.required],
    number: ['', [Validators.required, Validators.pattern('^[0-9() \\-]*$')]]
  });

  constructor(private readonly fb: FormBuilder,
              public ws: WebSocketConnectionService,
              private readonly router: Router,
              private userService: UserService) { }

  ngOnInit() {
    this.subscribeToUser();
    this.ws.socketConnection.sendDto(new ClientWantsToGetCountryCodeDto());
    this.subscribeToCountryCodes();
  }

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  subscribeToUser() {
    this.ws.user
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(u => {
        if (u) {
          this.user = u;

          this.form.patchValue({
            email: u.Email,
            firstName: u.FirstName,
            lastName: u.LastName,
            countryCode: u.CountryCode,
            number: u.Number
          });
        }
      });
  }

  private subscribeToCountryCodes() {
    this.ws.allCountryCodes.pipe(takeUntil(this.unsubscribe$)).subscribe(
      countryCodeList => {
        console.log(countryCodeList)
        if (countryCodeList) {
          this.allCountryCodes = countryCodeList!
        }
      });
  }

  editUser() {
    if(!this.form.valid) {
      //TODO toast error message
      return;
    }
    let userDto = new FullUserDto();
    userDto.Id = this.user!.Id;
    userDto.Email = this.form.controls.email.value!;
    userDto.FirstName = this.form.controls.firstName.value!;
    userDto.LastName = this.form.controls.lastName.value!;
    userDto.CountryCode = this.form.controls.countryCode.value!;
    userDto.Number = this.form.controls.number.value!;

    let dto = new ClientWantsToEditUserInfoDto({
      UserDto: userDto
    })

    this.userService.editUser(dto);

    this.router.navigate(['rooms/all']);
  }
}
