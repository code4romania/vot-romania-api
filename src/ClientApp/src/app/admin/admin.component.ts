import { Component, OnInit } from '@angular/core';
import { Logout } from '../state/auth';
import { Store } from '@ngrx/store';
import { ApplicationState } from '../state/reducers';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  constructor(private store: Store<ApplicationState>) {
  }

  public logout() {
    this.store.dispatch(new Logout());
  }

  public ngOnInit(): void {
  }

}
