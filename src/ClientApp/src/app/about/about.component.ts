import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { select, Store } from '@ngrx/store';
import { ApplicationState } from '../state/reducers';
import { getAboutInfo } from '../state/selectors';

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent implements OnInit {
  message$: Observable<string>;
  constructor(private store: Store<ApplicationState>) { }

  ngOnInit() {
    this.message$ = this.store.pipe(select(getAboutInfo));
  }


}
