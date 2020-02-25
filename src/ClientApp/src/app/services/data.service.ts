import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

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

export interface PollingStationDetails {
  judet: string;
  localitate: string;
  numarSectie: string;
  institutie: string;
  adresa: string;
}

export interface PollingStationInfo {
  id: string;
  lat: number;
  lng: number;
  properties: PollingStationDetails;
}

export interface ApplicationData {
  staticTexts: StaticData[];
  pollingStationsInfo: PollingStationInfo[];
}

@Injectable({ providedIn: 'root' })
export class DataService {
  constructor(private http: HttpClient) { }

  getData(): Observable<ApplicationData> {
    return this.http.get<ApplicationData>('api/data')
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
