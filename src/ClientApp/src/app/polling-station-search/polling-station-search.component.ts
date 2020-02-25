import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  AfterViewInit,
  OnDestroy
} from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { switchMap, debounceTime, map } from 'rxjs/operators';
import { HereAddressService, Suggestion } from '../services/here-suggest.service';
import { ApplicationState } from '../state/reducers';
import { Store, select } from '@ngrx/store';
import { getPollingStations } from '../state/selectors';

declare var H: any;

@Component({
  selector: 'app-polling-station-search',
  templateUrl: './polling-station-search.component.html',
  styleUrls: ['./polling-station-search.component.scss']
})
export class PollingStationSearchComponent implements OnInit, AfterViewInit, OnDestroy {
  control = new FormControl();
  filteredAddresses: Observable<Suggestion[]>;
  searchText: string = 'Caută adresa ta pentru a afla la ce secție ești arondat';

  private platform: any;
  private hereMap: any;
  private mapUi: any;

  @ViewChild('map', { static: true })
  public mapElement: ElementRef;

  private subscription: Subscription;

  constructor(private addressSuggest: HereAddressService, private store: Store<ApplicationState>) {
    this.platform = new H.service.Platform({
      apikey: hereMapsToken
    });
  }

  ngOnInit() {
    this.filteredAddresses = this.control.valueChanges.pipe(
      debounceTime(300),
      switchMap(value => this.addressSuggest.suggest(value)),
      map(value => value.suggestions)
    );

    this.initializeMap();

    this.store.pipe(select(getPollingStations))
      .subscribe(ps => {
        const icon = new H.map.Icon('assets/pin.png');
        const pinGroups = new H.map.Group();
        this.hereMap.addObject(pinGroups);

        pinGroups.addEventListener('tap', (evt) => {
          // event target is the marker itself, group is a parent event target
          // for all objects that it contains
          const bubble = new H.ui.InfoBubble(evt.target.getGeometry(), {
            // read custom data
            content: evt.target.getData()
          });
          // show info bubble
          this.mapUi.addBubble(bubble);
        }, false);

        ps.forEach(data => {
          const marker = new H.map.Marker({ lat: data.lat, lng: data.lng }, { icon: icon });
          marker.setData(data.properties.adresa);
          pinGroups.addObject(marker);
        });

      });
  }

  private initializeMap() {
    const defaultLayers = this.platform.createDefaultLayers();

    this.hereMap = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map,
      {
        center: { lat: 45.658, lng: 25.6012 },
        zoom: 7,
        pixelRatio: window.devicePixelRatio || 1
      });

    const behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));

    this.mapUi = H.ui.UI.createDefault(this.hereMap, defaultLayers);
  }

  ngAfterViewInit(): void {

  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  getDisplayFn() {
    return (val) => this.display(val);
  }

  private display(address: Suggestion): string {
    return address ? address.label : '';
  }

  onSelectingSuggestion(data: Suggestion): void {
    console.log('you selected:', data);
  }
}
