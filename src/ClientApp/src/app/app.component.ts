import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { LoadDataAction } from './state/actions';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {

  constructor(private store: Store<any>) {

  }
  title = 'app';
  ngOnInit(): void {
    this.store.dispatch(new LoadDataAction());
  }
}
