import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  AfterViewInit
} from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { switchMap, debounceTime, map } from 'rxjs/operators';
import { HereAddressService, Suggestion } from '../services/here-suggest.service';

declare var H: any;

@Component({
  selector: 'app-polling-station-search',
  templateUrl: './polling-station-search.component.html',
  styleUrls: ['./polling-station-search.component.scss']
})
export class PollingStationSearchComponent implements OnInit, AfterViewInit {
  control = new FormControl();
  filteredAddresses: Observable<Suggestion[]>;
  searchText: string = 'Caută adresa ta pentru a afla la ce secție ești arondat';

  private platform: any;

  @ViewChild('map', { static: true })
  public mapElement: ElementRef;

  constructor(private addressSuggest: HereAddressService) {
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
  }

  ngAfterViewInit(): void {
    const defaultLayers = this.platform.createDefaultLayers();

    const hereMap = new H.Map(
      this.mapElement.nativeElement,
      defaultLayers.vector.normal.map,
      {
        center: { lat: 45.658, lng: 25.6012 },
        zoom: 7,
        pixelRatio: window.devicePixelRatio || 1
      }
    );

    const behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(hereMap));

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
