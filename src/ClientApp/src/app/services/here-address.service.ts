import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ILocationSearchResponse {
  suggestions: AddressSuggestion[];
}

export interface AddressSuggestion {
  label: string;
  language: string;
  countryCode: string;
  locationId: string;
  address: Address;
  matchLevel: string;
}

export interface MetaInfo {
  timestamp: Date;
}

export interface DisplayPosition {
  latitude: number;
  longitude: number;
}

export interface NavigationPosition {
  latitude: number;
  longitude: number;
}

export interface TopLeft {
  latitude: number;
  longitude: number;
}

export interface BottomRight {
  latitude: number;
  longitude: number;
}

export interface MapView {
  topLeft: TopLeft;
  bottomRight: BottomRight;
}

export interface AdditionalData {
  value: string;
  key: string;
}

export interface Address {
  label: string;
  country: string;
  county: string;
  city: string;
  district: string;
  street: string;
  houseNumber: string;
  postalCode: string;
  additionalData: AdditionalData[];
}

export interface LocationDetails {
  locationId: string;
  locationType: string;
  displayPosition: DisplayPosition;
  navigationPosition: NavigationPosition[];
  mapView: MapView;
  address: Address;
}

export interface Result {
  relevance: number;
  matchLevel: string;
  matchType: string;
  location: LocationDetails;
}

export interface View {
  result: Result[];
  viewId: number;
}

export interface LocationDetails {
  metaInfo: MetaInfo;
  view: View[];
}

export interface LocationDetailsResponse {
  response: LocationDetails;
}

@Injectable({ providedIn: 'root' })
export class HereAddressService {

  private readonly suggestUrl = 'https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json';
  private readonly geocoderUrl = 'https://geocoder.ls.hereapi.com/6.2/geocode.json';

  constructor(private httpClient: HttpClient) { }

  suggest(location: string): Observable<ILocationSearchResponse> {
    let params = new HttpParams();

    params = params.append('query', location);
    params = params.append('maxresults', '5');
    params = params.append('apikey', hereMapsToken);

    return this.httpClient.get<ILocationSearchResponse>(this.suggestUrl, { params: params });
  }

  getLocationDetails(locationId: string): Observable<LocationDetailsResponse> {
    let params = new HttpParams();

    params = params.append('locationid', locationId);
    params = params.append('jsonattributes', '1');
    params = params.append('gen', '1');
    params = params.append('apikey', hereMapsToken);

    return this.httpClient.get<LocationDetailsResponse>(this.geocoderUrl, { params: params });
  }

}
