import { Action } from '@ngrx/store';
import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Observable, of } from 'rxjs';
import { AuthService, LoginCredentials, LoginResponse } from '../services/auth.service';
import { catchError, map, mergeMap, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { actionType } from 'src/app/state/actions';

export class AuthActionTypes {
  static readonly LOGIN_REQUESTED = actionType('Login requested');
  static readonly LOGIN_SUCCEEDED = actionType('Login succeeded');
  static readonly LOGIN_FAILED = actionType('Login failed');
  static readonly LOGOUT = actionType('Logout');
}

export class LoginRequested implements Action {
  readonly type = AuthActionTypes.LOGIN_REQUESTED;

  constructor(public payload: LoginCredentials) {
    this.payload = payload;
  }
}

export class LoginSucceeded implements Action {
  readonly type = AuthActionTypes.LOGIN_SUCCEEDED;

  constructor(public payload: LoginResponse) {
    this.payload = payload;
  }
}

export class LoginFailed implements Action {
  readonly type = AuthActionTypes.LOGIN_FAILED;

  constructor(public payload: string) {
  }
}

export class Logout implements Action {
  readonly type = AuthActionTypes.LOGOUT;

  constructor() {
  }
}

export type AuthActions = LoginRequested | LoginSucceeded | LoginFailed | Logout;

@Injectable({providedIn: 'root'})
export class AuthEffects {

  constructor(private actions$: Actions,
              private authService: AuthService,
              private router: Router) {

  }

  @Effect()
  login$: Observable<Action> = this.actions$.pipe(
    ofType(AuthActionTypes.LOGIN_REQUESTED),
    mergeMap((action: LoginRequested) =>
      this.authService.login(action.payload).pipe(
        map(res => (new LoginSucceeded(res))),
        catchError(err => {
          return of(new LoginFailed(err));
        })
      )
    )
  );

  @Effect({dispatch: false})
  loginSucceeded$: Observable<any> = this.actions$.pipe(
    ofType(AuthActionTypes.LOGIN_SUCCEEDED),
    tap((action: LoginSucceeded) => {
      this.router.navigate(['admin']);
      localStorage.setItem('token', action.payload.token);
    })
  );

  @Effect({dispatch: false})
  logout$: Observable<any> = this.actions$.pipe(
    ofType(AuthActionTypes.LOGOUT),
    tap(() => {
      this.router.navigate(['admin/login']);
      localStorage.removeItem('token');
    })
  );
}
