import { Component } from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {select, Store} from '@ngrx/store';
import {LoginRequested} from '../state/auth';
import {getAuthError, getAuthToken} from '../state/selectors';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  public form: FormGroup;
  public authError: string;

  constructor(private store: Store<any>) {
    this.store.pipe(select(getAuthError))
      .subscribe(authError => {
        this.authError = authError;
      });

    this.form = new FormGroup({
      username: new FormControl('', [
        Validators.required,
      ]),
      password: new FormControl('', [
        Validators.required,
      ]),
    });
  }

  async login() {
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    this.store.dispatch(new LoginRequested(this.form.value));
  }

}
