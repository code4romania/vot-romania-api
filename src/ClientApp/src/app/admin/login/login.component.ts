import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { select, Store } from '@ngrx/store';
import { getError } from '../../state/selectors';
import { LoginRequested } from '../state/auth';
import { ClearErrorAction } from '../state/admin-actions';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  public form: FormGroup;
  public error: string;

  constructor(private store: Store<any>) {
    this.store.dispatch(new ClearErrorAction());

    this.store.pipe(select(getError))
      .subscribe(error => {
        this.error = error;
      });

    this.form = new FormGroup({
      userName: new FormControl('', [
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
