import { Component, OnInit } from '@angular/core';
import {LoginRequested, Logout} from '../state/auth';
import {Store} from '@ngrx/store';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {

  constructor(private store: Store<any>) { }

  ngOnInit() {
  }

  logout() {
    this.store.dispatch(new Logout());
  }

}
