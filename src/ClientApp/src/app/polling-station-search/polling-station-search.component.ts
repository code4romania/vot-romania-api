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
import { switchMap, debounceTime, map, filter } from 'rxjs/operators';
import { HereAddressService, AddressSuggestion, LocationDetails } from '../services/here-address.service';
import { ApplicationState } from '../state/reducers';
import { Store, select } from '@ngrx/store';
import { getMapPins } from '../state/selectors';
import { replace } from 'lodash';
import { PollingStationGroup } from '../services/data.service';
import { LoadLocations } from '../state/actions';

declare var H: any;
export enum PinType {
  UserLocationIcon = '#efc007',
  PollingStationIcon = '#119DA4'
}
@Component({
  selector: 'app-polling-station-search',
  templateUrl: './polling-station-search.component.html',
  styleUrls: ['./polling-station-search.component.scss']
})
export class PollingStationSearchComponent implements OnInit, AfterViewInit, OnDestroy {
  private readonly svgIcon: string = `<svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="%%fill%%" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-map-pin"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3" fill="%%fill%%" ></circle></svg>`;
  private readonly userIcon = new H.map.Icon(this.getSvgMarker(PinType.UserLocationIcon));
  private readonly pollingStationIcon = new H.map.Icon(this.getSvgMarker(PinType.PollingStationIcon));

  control = new FormControl();
  filteredAddresses: Observable<AddressSuggestion[]>;
  searchText: 'Caută adresa ta pentru a afla la ce secție ești arondat';

  private platform: any;
  private hereMap: any;
  private mapUi: any;
  private currentlyOpenedInfoBubble: any;


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
      filter(value => typeof value === 'string'),
      switchMap(value => this.addressSuggest.suggest(value)),
      map(value => value.suggestions)
    );

    this.initializeMap();

    this.store.pipe(select(getMapPins))
      .pipe(filter(data => data !== undefined && data.pollingStations !== undefined && data.userAddress !== undefined))
      .subscribe((details: { userAddress: LocationDetails, pollingStations: PollingStationGroup[] }) => {
        this.clearMap();
        const { userAddress, pollingStations } = details;

        const position = userAddress.displayPosition;
        const userAddressMarker = new H.map.Marker({ lat: position.latitude, lng: position.longitude }, { icon: this.userIcon });
        userAddressMarker.setData('locatia ta');
        const mapMarkers: any[] = [];
        mapMarkers.push(userAddressMarker);

        pollingStations.forEach(p => {
          const pollingStationMarker = new H.map.Marker({ lat: p.latitude, lng: p.longitude }, { icon: this.pollingStationIcon });
          pollingStationMarker.setData(this.getPollingStationinfoBubble(p));
          mapMarkers.push(pollingStationMarker);
        });

        const group = new H.map.Group();
        group.addEventListener('tap', (evt) => {
          // event target is the marker itself, group is a parent event target
          // for all objects that it contains
          const bubble = new H.ui.InfoBubble(evt.target.getGeometry(), {
            // read custom data
            content: evt.target.getData()
          });
          this.mapUi.removeBubble(this.currentlyOpenedInfoBubble);
          // show info bubble
          this.mapUi.addBubble(bubble);
          this.currentlyOpenedInfoBubble = bubble;
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

  getPollingStationinfoBubble(group: PollingStationGroup): string {
    // TODO: style info bubble text
    return group.pollingStations.reduce((accumulator, currentValue) =>
      accumulator + `<div style="width: 472px;">
     <div style="font-family: Titillium Web;font-style: normal;font-weight: 600;font-size: 34px;line-height: 52px;display: flex;align-items: center;text-align: center;"> Sectia de votare ${currentValue.pollingStationNumber},  ${currentValue.locality}</div> 
      <div style="font-family: Titillium Web;font-style: normal;font-weight: 600;font-size: 18px;line-height: 27px;display: flex;align-items: center;text-align: center;">
       <p style="background: #FFCC00;font-family: Titillium Web;
      font-style: normal;font-weight: normal;font-size: 14px;line-height: 21px;">Adresa:</p>
      <u>${currentValue.address}</u></div>
      <br>  <div style="width: 431px;height: 1px;left: 15px;top: 353px;background: #C4C4C4;"></div>` + '</div>'
      , '');
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

    const behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.hereMap));
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
