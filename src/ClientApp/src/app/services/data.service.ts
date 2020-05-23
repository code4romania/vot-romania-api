import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { throwError, Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface Option {
  title: string;
  description: string;
}
export interface VotingGuide {
  description: string;
  options: Option[];
}

export interface StaticData {
  language: string;
  generalInfo: string;
  votersGuide: VotingGuide;
}

export interface PollingStation {
  id: number;
  latitude: number;
  longitude: number;
  county: string;
  locality: string;
  pollingStationNumber: string;
  institution: string;
  address: string;
  assignedAddresses: string[];
}

export interface PollingStationGroup {
  latitude: number;
  longitude: number;
  pollingStations: PollingStation[];
}

export interface ApplicationData {
  content: StaticData[];
}

@Injectable({ providedIn: 'root' })
export class DataService {
  constructor(private http: HttpClient) { }

  getData(): Observable<ApplicationData> {
    return this.http.get<ApplicationData>('/api/application-content')
      .pipe(catchError(this.handleError));
  }

  getPollingStations(latitude: number, longitude: number): Observable<PollingStationGroup[]> {
    let params = new HttpParams();

    params = params.append('latitude', latitude.toString());
    params = params.append('longitude', longitude.toString());

    return this.http.get<PollingStationGroup[]>('api/polling-station/near-me', { params: params })
      .pipe(catchError(this.handleError));
  }

  private handleError(err) {
    // in a real world app, we may send the server to some remote logging infrastructure
    // instead of just logging it to the console
    let errorMessage: string;
    if (err.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      errorMessage = `An error occurred: ${err.error.message}`;
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      errorMessage = `Backend returned code ${err.status}: ${err.body.error}`;
    }
    console.error(err);
    return throwError(errorMessage);
  }
}
