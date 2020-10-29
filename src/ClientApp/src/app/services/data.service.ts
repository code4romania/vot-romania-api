import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ErrorService } from './error.service';
import { RequestHttpParams } from './params-builder';

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
  locality: string;
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
  distance?: number;
}

export interface PaginationDetails {
  pageSize?: number;
  pageNumber?: number;
}

export interface AssignedAddress {
  streetCode: string;
  street: string;
  houseNumbers: string;
  remarks: string;
}

export interface PollingStationGroup {
  latitude: number;
  longitude: number;
  distance: number;
  pollingStations: PollingStation[];
}

export interface ApplicationData {
  content: StaticData[];
}

export interface PaginatedResponse<T> {
  results: T[];
  currentPage: number;
  pageCount: number;
  pageSize: number;
  rowCount: number;
}

export interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  detail: string;
  instance: string;
}

@Injectable({ providedIn: 'root' })
export class DataService {
  constructor(private http: HttpClient,
    private errorService: ErrorService) { }

  getData(): Observable<ApplicationData> {
    return this.http.get<ApplicationData>('/api/application-content')
      .pipe(catchError(this.errorService.handleError));
  }

  getPollingStations(latitude: number, longitude: number): Observable<PollingStationGroup[]> {
    let params = RequestHttpParams
      .create()
      .append('latitude', latitude.toString())
      .append('longitude', longitude.toString())
      .please();

    return this.http.get<PollingStationGroup[]>('api/polling-station/near-me', { params: params },)
      .pipe(catchError(this.errorService.handleError));
  }
}
