import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Logout } from './state/auth';
import { AdminState } from './state/admin-reducers';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  constructor(private store: Store<AdminState>) {
  }

  public logout() {
    this.store.dispatch(new Logout());
  }

  public ngOnInit(): void {
  }

}
