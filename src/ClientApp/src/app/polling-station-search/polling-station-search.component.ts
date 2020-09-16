import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  AfterViewInit,
  OnDestroy,
  ViewEncapsulation
} from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable, Subscription, Subject } from 'rxjs';
import { switchMap, debounceTime, map, filter } from 'rxjs/operators';
import { HereAddressService, AddressSuggestion, LocationDetails } from '../services/here-address.service';
import { ApplicationState } from '../state/reducers';
import { Store, select } from '@ngrx/store';
import { getMapPins } from '../state/selectors';
import { PollingStationGroup, PollingStation } from '../services/data.service';
import { LoadLocations } from '../state/actions';
import { PollingStationMatcherService } from '../services/polling-station-matcher.service';
import { intersectionBy } from 'lodash';

declare var H: any;
@Component({
  selector: 'app-polling-station-search',
  templateUrl: './polling-station-search.component.html',
  styleUrls: ['./polling-station-search.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class PollingStationSearchComponent implements OnInit, AfterViewInit, OnDestroy {
  private readonly userIcon = new H.map.Icon(this.getHouseMarker());
  private readonly pollingStationIcon = new H.map.Icon(this.getPollingStationMarker());
  control = new FormControl();
  filteredAddresses: Observable<AddressSuggestion[]>;
  pollingStations: PollingStation[];
  pollingStationsForAddress: PollingStation[];
  pollingStatationsGroup$: Subject<PollingStationGroup> = new Subject<PollingStationGroup>();

  private platform: any;
  private hereMap: any;
  private behavior: any;
  // @ts-ignore
  private mapUi: any;


  @ViewChild('map', { static: true })
  public mapElement: ElementRef;

  private subscription: Subscription;

  constructor(private addressSuggest: HereAddressService,
    private store: Store<ApplicationState>,
    private pollingStationMatcher: PollingStationMatcherService) {
    this.platform = new H.service.Platform({
      apikey: hereMapsToken
    });
  }

  ngOnInit() {
    this.filteredAddresses = this.control.valueChanges.pipe(
      debounceTime(300),
      filter(value => typeof value === 'string'),
      switchMap(value => this.addressSuggest.suggest(value)),
      map(value => value.suggestions)
    );

    this.initializeMap();

    this.store.pipe(select(getMapPins))
      .pipe(filter(data => data !== undefined && data.pollingStations !== undefined && data.userAddress !== undefined))
      .subscribe((details: { userAddress: LocationDetails, pollingStations: PollingStationGroup[] }) => {
        this.clearMap();
        const userAddress = details.userAddress;
        let pollingStationsGroups = details.pollingStations;

        const position = userAddress.displayPosition;
        const userAddressMarker = new H.map.Marker({ lat: position.latitude, lng: position.longitude }, { icon: this.userIcon });
        const mapMarkers: any[] = [];
        this.hereMap.addObject(userAddressMarker);

        this.pollingStations = [].concat(...pollingStationsGroups.map(g => g.pollingStations.map(ps => ({ ...ps, distance: g.distance }))));
        this.pollingStationsForAddress = this.pollingStationMatcher.findPollingStation(this.pollingStations, userAddress.address);

        if (this.pollingStationsForAddress !== undefined && this.pollingStationsForAddress.length !== 0) {

          pollingStationsGroups = pollingStationsGroups
            .filter(g => intersectionBy(g.pollingStations, this.pollingStationsForAddress, 'id').length > 0);

          pollingStationsGroups.forEach(g => {
            g.pollingStations = intersectionBy(g.pollingStations, this.pollingStationsForAddress, 'id');
          });
        }

        pollingStationsGroups.forEach(pollingStationGroup => {
          const pollingStationMarker = new H.map.Marker(
            {
              lat: pollingStationGroup.latitude,
              lng: pollingStationGroup.longitude
            },
            {
              icon: this.pollingStationIcon
            });

          pollingStationMarker.setData(pollingStationGroup);
          mapMarkers.push(pollingStationMarker);
        });

        const group = new H.map.Group();
        group.addEventListener('tap', (evt) => {
          // read custom data
          const groupDetails: PollingStationGroup = evt.target.getData();
          this.pollingStatationsGroup$.next(groupDetails);
        }, false);

        // add markers to the group
        group.addObjects(mapMarkers);
        this.hereMap.addObject(group);

        if (mapMarkers.length > 1) {
          // look at whole group of polling stations
          this.hereMap.getViewModel().setLookAtData({
            bounds: group.getBoundingBox(),
          });
        } else if (mapMarkers.length === 0) {
          // look at user location
          this.hereMap.setCenter({ lat: position.latitude, lng: position.longitude });
          this.hereMap.setZoom(16);
        } else {
          // look at assigned polling station
          this.hereMap.setCenter({ lat: pollingStationsGroups[0].latitude, lng: pollingStationsGroups[0].longitude });
          this.hereMap.setZoom(16);
        }

      });
  }

  private initializeMap() {
    const pixelRatio = window.devicePixelRatio || 1;
    const defaultLayers = this.platform.createDefaultLayers({
      tileSize: pixelRatio === 1 ? 256 : 512,
      ppi: pixelRatio === 1 ? undefined : 320
    });

    this.hereMap = new H.Map(this.mapElement.nativeElement,
      defaultLayers.vector.normal.map,
      {
        center: { lat: 45.658, lng: 25.6012 },
        zoom: 7,
        pixelRatio: pixelRatio
      });

    this.behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
    this.behavior.disable(H.mapevents.Behavior.WHEELZOOM);

    this.mapUi = H.ui.UI.createDefault(this.hereMap, defaultLayers);
  }

  private clearMap(): void {
    this.hereMap.removeObjects(this.hereMap.getObjects());
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

  private display(address: AddressSuggestion): string {
    return address ? address.label : '';
  }

  onSelectingSuggestion(data: AddressSuggestion): void {
    this.store.dispatch(new LoadLocations(data.locationId));
  }

  private getHouseMarker(): string {
    return `<svg version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" width="48" height="48" viewBox="0 0 86.68 100" style="enable-background:new 0 0 86.68 100;" xml:space="preserve"><style type="text/css">.st0{fill:#342245;}.st1{fill:#E9BF1E;}</style><g><g><g><g><path class="st0" d="M43.34,0C19.44,0,0,18.66,0,41.59c0,31.12,39.77,56.79,41.46,57.86c0.57,0.37,1.23,0.55,1.88,0.55 c0.65,0,1.31-0.18,1.88-0.55c1.69-1.08,41.46-26.74,41.46-57.86C86.68,18.66,67.24,0,43.34,0z M43.34,92.26 C35.64,86.9,7.01,65.35,7.01,41.59c0-19.07,16.3-34.58,36.33-34.58s36.33,15.51,36.33,34.58C79.67,65.32,51.03,86.9,43.34,92.26 z"/></g></g></g><path class="st1" d="M7.01,41.59c0,23.76,28.63,45.32,36.33,50.67c7.69-5.37,36.33-26.95,36.33-50.67 c0-19.07-16.3-34.58-36.33-34.58S7.01,22.52,7.01,41.59z"/><g><g><g><path class="st0" d="M43.34,32.33l-18.32,15.1c0,0.02-0.01,0.05-0.02,0.1c-0.01,0.04-0.02,0.07-0.02,0.1v15.29 c0,0.55,0.2,1.03,0.61,1.43c0.4,0.4,0.88,0.61,1.43,0.61h12.23V52.72h8.16v12.23h12.23c0.55,0,1.03-0.2,1.43-0.61 c0.4-0.4,0.61-0.88,0.61-1.43V47.62c0-0.08-0.01-0.15-0.03-0.19L43.34,32.33z"/><path class="st0" d="M68.67,40.57l-6.98-5.8v-13c0-0.3-0.1-0.54-0.29-0.73c-0.19-0.19-0.43-0.29-0.73-0.29h-6.12 c-0.3,0-0.54,0.1-0.73,0.29c-0.19,0.19-0.29,0.44-0.29,0.73v6.21l-7.77-6.5c-0.68-0.55-1.49-0.83-2.42-0.83 c-0.93,0-1.74,0.28-2.42,0.83L18.01,40.57c-0.21,0.17-0.33,0.4-0.35,0.68c-0.02,0.29,0.05,0.54,0.22,0.75l1.98,2.36 c0.17,0.19,0.39,0.31,0.67,0.35c0.25,0.02,0.51-0.05,0.76-0.22L43.34,26.1l22.05,18.38c0.17,0.15,0.39,0.22,0.67,0.22h0.1 c0.28-0.04,0.5-0.16,0.67-0.35L68.8,42c0.17-0.21,0.24-0.46,0.22-0.75C69,40.96,68.88,40.74,68.67,40.57z"/></g></g></g></g></svg>`;
  }
  private getPollingStationMarker(): string {
    return `<svg version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" width="48" height="48" viewBox="0 0 86.68 100" style="enable-background:new 0 0 86.68 100;" xml:space="preserve"><style type="text/css">.st0{fill:#342245;}.st1{fill:#E9BF1E;}</style><path class="st0" d="M43.34,0C19.44,0,0,18.66,0,41.59c0,31.12,39.77,56.79,41.46,57.86c0.57,0.37,1.23,0.55,1.88,0.55 c0.65,0,1.31-0.18,1.88-0.55c1.69-1.08,41.46-26.74,41.46-57.86C86.68,18.66,67.24,0,43.34,0z"/><g><path class="st1" d="M69.61,32.67c-0.2-0.49-0.6-0.89-1.09-1.09L34.01,17.28c-1.03-0.43-2.21,0.06-2.64,1.09L17.07,52.88 c-0.21,0.49-0.21,1.05,0,1.54c0.2,0.49,0.6,0.89,1.09,1.09l23.17,9.6c0.25,0.1,0.51,0.15,0.77,0.15c0.26,0,0.53-0.05,0.77-0.15 l16.03-6.64c0.49-0.2,0.89-0.6,1.09-1.09l9.6-23.17C69.81,33.72,69.81,33.16,69.61,32.67z M45.84,59.52l2.06-4.97l4.97,2.06 L45.84,59.52z M57.05,53.97l-9.47-3.92c-1.03-0.43-2.21,0.06-2.64,1.09l-3.92,9.47l-19.45-8.05l12.75-30.78l30.78,12.75 L57.05,53.97z"/><polygon class="st1" points="52.82,41.16 51.27,37.44 40.94,41.72 39.18,37.48 35.45,39.03 38.75,46.99 	"/></g></svg>`;
  }
}
