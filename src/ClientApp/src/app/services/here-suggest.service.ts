import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
export interface ILocationSearchResponse {
  suggestions: Suggestion[];
}

export interface Address {
  country: string;
  county: string;
  city: string;
  district: string;
  postalCode: string;
  state: string;
  street: string;
}

export interface Suggestion {
  label: string;
  language: string;
  countryCode: string;
  locationId: string;
  address: Address;
  matchLevel: string;
}

@Injectable({ providedIn: 'root' })
export class HereAddressService {

  private readonly url = 'https://autocomplete.geocoder.ls.hereapi.com/6.2/suggest.json';

  constructor(private httpClient: HttpClient) { }

  suggest(location: string): Observable<ILocationSearchResponse> {
    let params = new HttpParams();

    // Begin assigning parameters
    params = params.append('query', location);
    params = params.append('maxresults', '5');
    params = params.append('apikey', hereMapsToken);

    return this.httpClient.get<ILocationSearchResponse>(this.url, { params: params });
  }

}
