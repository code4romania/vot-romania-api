import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { ErrorService } from './error.service';

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

export interface AssignedAddress {
  houseNumbers: string;
  remarks: string;
  street: string;
  streetCode: string;
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
  assignedAddresses: AssignedAddress[];
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
  constructor(private http: HttpClient,
              private authService: AuthService,
              private errorService: ErrorService) { }

  getData(): Observable<ApplicationData> {
    return this.http.get<ApplicationData>('/api/application-content')
      .pipe(catchError(this.errorService.handleError));
  }

  updateData(data: StaticData): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}`});
    return this.http.post<any>(`/api/application-content/${data.language}`, data, { headers })
      .pipe(catchError(this.errorService.handleError));
  }

  getPollingStations(latitude: number, longitude: number): Observable<PollingStationGroup[]> {
    let params = new HttpParams();

    params = params.append('latitude', latitude.toString());
    params = params.append('longitude', longitude.toString());

    return this.http.get<PollingStationGroup[]>('api/polling-station/near-me', { params: params })
      .pipe(catchError(this.errorService.handleError));
  }
}
