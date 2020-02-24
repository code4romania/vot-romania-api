import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApplicationState } from '../state/reducers';
import { Store, select } from '@ngrx/store';
import { Observable, Subscription } from 'rxjs';
import { getVotingGuide } from '../state/selectors';
import { VotingGuide } from '../services/data.service';
import { switchMap, map, take, tap } from 'rxjs/operators';
import { ofType } from '@ngrx/effects';

export interface Tile {
  title: string;
  description: string;
  isSelected: boolean;
}
export interface VotingGuideViewModel {
  description: string;
  options: Tile[];
}
@Component({
  selector: 'app-voters-guide',
  templateUrl: './voters-guide.component.html',
  styleUrls: ['./voters-guide.component.scss']
})
export class VotersGuideComponent implements OnInit, OnDestroy {

  description: string;

  data: VotingGuideViewModel;
  subscription: Subscription;

  constructor(private store: Store<ApplicationState>) {

  }

  ngOnInit(): void {
    this.subscription = this.store
      .pipe(select(getVotingGuide),
        map(guide => {
          if (guide === undefined) {
            return undefined;
          }

          return {
            description: guide.description,
            options: guide.options.map(o => ({ ...o, isSelected: false }))
          };
        }))
      .subscribe(x => {
        this.data = x;
      });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  showDescriptionFor(tile: Tile): void {
    this.description = tile.description;
    this.data.options.forEach(o => o.isSelected = false);
    tile.isSelected = true;
  }
}
