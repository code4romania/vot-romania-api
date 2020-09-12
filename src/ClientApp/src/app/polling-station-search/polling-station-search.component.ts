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
import { replace } from 'lodash';
import { PollingStationGroup, PollingStation } from '../services/data.service';
import { LoadLocations } from '../state/actions';
import { PollingStationMatcherService } from '../services/polling-station-matcher.service';

declare var H: any;
export enum PinType {
  UserLocationIcon = '#efc007',
  PollingStationIcon = '#119DA4'
}
@Component({
  selector: 'app-polling-station-search',
  templateUrl: './polling-station-search.component.html',
  styleUrls: ['./polling-station-search.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class PollingStationSearchComponent implements OnInit, AfterViewInit, OnDestroy {
  private readonly svgIcon: string = `<svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="%%fill%%" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-map-pin"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3" fill="%%fill%%" ></circle></svg>`;
  private readonly userIcon = new H.map.Icon(this.getSvgMarker(PinType.UserLocationIcon));
  private readonly pollingStationIcon = new H.map.Icon(this.getSvgMarker(PinType.PollingStationIcon));

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
        const { userAddress, pollingStations: pollingStationsGroups } = details;

        const position = userAddress.displayPosition;
        const userAddressMarker = new H.map.Marker({ lat: position.latitude, lng: position.longitude }, { icon: this.userIcon });
        userAddressMarker.setData('locatia ta');
        const mapMarkers: any[] = [];
        mapMarkers.push(userAddressMarker);
        this.pollingStations = [].concat(...pollingStationsGroups.map(g => g.pollingStations.map(ps => ({ ...ps, distance: g.distance }))));
        this.pollingStationsForAddress = this.pollingStationMatcher.findPollingStation(this.pollingStations, userAddress.address);
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

        // get geo bounding box for the group and set it to the map
        this.hereMap.getViewModel().setLookAtData({
          bounds: group.getBoundingBox()
        });
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

  private getSvgMarker(pinType: PinType): string {
    return replace(this.svgIcon, '%%fill%%', pinType);
  }
}
