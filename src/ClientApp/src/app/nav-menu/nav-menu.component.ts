import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { ApplicationState } from '../state/reducers';
import { Store, select } from '@ngrx/store';
import { getLanguages, getSelectedLanguage } from '../state/selectors';
import { ChangeSelectedLanguage } from '../state/actions';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {
  selectedLanguage$: Observable<string>;
  availableLanguages$: Observable<string[]>;

  constructor(private store: Store<ApplicationState>, private translate: TranslateService) {
    this.availableLanguages$ = this.store.pipe(select(getLanguages));
    this.selectedLanguage$ = this.store.pipe(select(getSelectedLanguage));
  }

  changeSelectedLanguageTo(language: string): void {
    this.store.dispatch(new ChangeSelectedLanguage(language));
    this.translate.use(language.toLocaleLowerCase());
  }
}
