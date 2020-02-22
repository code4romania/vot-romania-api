import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ApplicationState } from '../state/reducers';
import { Store, select } from '@ngrx/store';
import { getGeneralInfo } from '../state/selectors';

@Component({
  selector: 'app-general-info',
  templateUrl: './general-info.component.html',
  styleUrls: ['./general-info.component.scss']
})
export class GeneralInfoComponent implements OnInit {
  message$: Observable<string>;
  constructor(private store: Store<ApplicationState>) { }

  ngOnInit() {
    this.message$ = this.store.pipe(select(getGeneralInfo));
  }

}
