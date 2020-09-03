import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApplicationState } from '../state/reducers';
import { Store, select } from '@ngrx/store';
import { Subscription, Observable } from 'rxjs';
import { getVotingGuide } from '../state/selectors';
import { map } from 'rxjs/operators';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { VotersDecisionTreeService, OperatorTreeNode } from '../services/voters-decision-tree.service';

export interface Tile {
  title: string;
  description: string;
}

export interface VotingGuideViewModel {
  description: string;
  options: Tile[];
}

@Component({
  selector: 'app-voters-guide',
  templateUrl: './voters-guide.component.html',
  styleUrls: ['./voters-guide.component.scss'],
  animations: [
    trigger('flyIn', [
      state('in', style({ transform: 'translateX(0)' })),
      transition(':enter', [
        style({ transform: 'translateX(-100%)' }),
        animate(250)
      ])
    ])
  ]
})
export class VotersGuideComponent implements OnInit, OnDestroy {
  description: string;
  title: string;

  data: VotingGuideViewModel;
  subscription: Subscription;
  currentSentence$: Observable<string[]> = this.votersDecisionTreeService.currentSentence$;
  options$: Observable<OperatorTreeNode[]> = this.votersDecisionTreeService.options$;
  isBeyondInitialQuestion$: Observable<boolean> = this.votersDecisionTreeService.isBeyondInitialQuestion$;
  hasError$: Observable<boolean> = this.votersDecisionTreeService.hasError$;

  constructor(private store: Store<ApplicationState>,
    private votersDecisionTreeService: VotersDecisionTreeService) {
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
    this.startOver();

  }

  selectOption(optionId: string): void {
    this.votersDecisionTreeService.selectOption(optionId);
  }

  back(): void {
    this.votersDecisionTreeService.back();
  }

  startOver(): void {
    this.votersDecisionTreeService.startOver();
  }
}
