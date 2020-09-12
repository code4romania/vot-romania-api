import { Component, OnInit, Input } from '@angular/core';
import { Subject } from 'rxjs';
import { PollingStationGroup, PollingStation } from 'src/app/services/data.service';
import { filter } from 'rxjs/operators';
import { trigger, style, transition, animate } from '@angular/animations';

@Component({
  selector: 'app-map-ps-details',
  templateUrl: './map-ps-details.component.html',
  styleUrls: ['./map-ps-details.component.scss'],
  animations: [
    trigger(
      'inOutAnimation',
      [
        transition(
          ':enter',
          [
            style({ opacity: 0 }),
            animate('150ms ease-out',
              style({ opacity: 1 }))
          ]
        ),
        transition(
          ':leave',
          [
            style({ opacity: 1 }),
            animate('150ms ease-in',
              style({ opacity: 0 }))
          ]
        )
      ]
    )
  ]
})
export class MapPsDetailsComponent implements OnInit {
  pollingStations: PollingStation[];
  isVisible: boolean;

  @Input() dataSource: Subject<PollingStationGroup>;

  constructor() { }

  ngOnInit() {
    this.dataSource
      .pipe(filter(data => data !== undefined && data.pollingStations !== undefined))
      .subscribe(group => {
        this.isVisible = true;
        this.pollingStations = [].concat(group.pollingStations.map(ps => ({ ...ps, distance: group.distance })));
      });
  }

  close(): void {
    this.isVisible = false;
  }


  trackFn(index: number, item: PollingStation): number {
    if (item.id) {
      return item.id;
    }

    return index;
  }
}
