import { Component } from '@angular/core';
import { ApplicationState } from '../state/reducers';
import { Store, select } from '@ngrx/store';
import { Observable } from 'rxjs';
import { getGeneralInfo, getVotingGuide } from '../state/selectors';
import { VotingGuide } from '../services/data.service';
export interface Tile {
  title: string;
  description: string;
}

@Component({
  selector: 'app-voters-guide',
  templateUrl: './voters-guide.component.html',
  styleUrls: ['./voters-guide.component.scss']
})
export class VotersGuideComponent {
  description: string;

  data$: Observable<VotingGuide>;
  
  constructor(private store: Store<ApplicationState>) {
    this.data$ = this.store.pipe(select(getVotingGuide));
  }

  showDescriptionFor(tile: Tile): void {
    this.description = tile.description;
  }
}
