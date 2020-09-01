import { Component, OnInit, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import { ApplicationState } from '../state/reducers';
import { Store, select } from '@ngrx/store';
import { Subscription } from 'rxjs';
import { getVotingGuide } from '../state/selectors';
import { map } from 'rxjs/operators';

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
  @ViewChild('optionTitle', {static: true}) optionTitle: ElementRef;

  description: string;
  title: string;

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

  showDescriptionFor(tile: Tile, targetElement: HTMLElement): void {
    this.title = tile.title;
    this.description = tile.description;
    this.data.options.forEach(o => o.isSelected = false);
    tile.isSelected = true;
    targetElement.scrollIntoView({ behavior: 'smooth', block: 'start' });

  }
}
