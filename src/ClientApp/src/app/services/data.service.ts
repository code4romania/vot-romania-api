import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

import { throwError, Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
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

export interface PaginationDetails {
  pageSize?: number;
  pageNumber?: number;
}

export interface ImportedPollingStationsFilter {
  county?: string;
  locality?: string;
  pollingStationNumber?: string;
  institution?: string;
  address?: string;
  resolvedAddressStatus?: string;
}

export interface ImportedPollingStation {
  id: number;
  latitude: number;
  longitude: number;
  county: string;
  locality: string;
  pollingStationNumber: string;
  institution: string;
  address: string;
  resolvedAddressStatus: string; // change to enum
  failMessage: string;
  assignedAddresses: AssignedAddress[];
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

export interface ImportJobDetails {
  jobId: string;
  fileName: string
  status: string;
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
    private authService: AuthService,
    private errorService: ErrorService) { }

  getData(): Observable<ApplicationData> {
    return this.http.get<ApplicationData>('/api/application-content')
      .pipe(catchError(this.errorService.handleError));
  }

  updateData(data: StaticData): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
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

  getImportedPollingStations(jobId: string, filter: ImportedPollingStationsFilter, pagination: PaginationDetails): Observable<PaginatedResponse<ImportedPollingStation>> {
    const params = RequestHttpParams
      .create()
      .append('address', filter.address)
      .append('county', filter.county)
      .append('institution', filter.institution)
      .append('locality', filter.locality)
      .append('pollingStationNumber', filter.pollingStationNumber)
      .append('resolvedAddressStatus', filter.resolvedAddressStatus)
      .append('pageNumber', pagination.pageNumber)
      .append('pageSize', pagination.pageSize)
      .please();

    return this.http.get<PaginatedResponse<ImportedPollingStation>>(`/api/admin/import/${jobId}/imported-polling-stations`, { params: params })
      .pipe(catchError(this.errorService.handleError));
  }

  getImportJobDetails(): Observable<ImportJobDetails> {
    return this.http.get<ImportJobDetails>('/api/admin/import/current-job')
      .pipe(catchError(this.errorService.handleError));
  }

  deleteImportedPollingStationId(jobId: string, importedPollingStationId: number): Observable<any> {
    return this.http.delete<PaginatedResponse<ImportedPollingStation>>(`/api/admin/import/${jobId}/imported-polling-stations/${importedPollingStationId}`)
      .pipe(catchError(this.errorService.handleError));
  }

  addImportedPollingStation(jobId: string, importedPollingStation: ImportedPollingStation, adddresses: AssignedAddress[]): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    const data = {
      ...importedPollingStation,
      assignedAddresses: adddresses
    }
    return this.http.post(`/api/admin/import/${jobId}/imported-polling-stations`, data, { headers })
      .pipe(catchError(this.errorService.handleError));
  }

  updateImportedPollingStation(jobId: string, pollingStationId: number, importedPollingStation: ImportedPollingStation, adddresses: AssignedAddress[]): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    const data = {
      ...importedPollingStation,
      assignedAddresses: adddresses
    }
    return this.http.post(`/api/admin/import/${jobId}/imported-polling-stations/${pollingStationId}`, data, { headers })
      .pipe(catchError(this.errorService.handleError));
  }

  restartJob(jobId: string): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    return this.http.post(`/api/admin/import/restart-job/${jobId}`, { headers })
      .pipe(catchError(this.errorService.handleError));
  }

  cancelJob(jobId: string): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    return this.http.post(`/api/admin/import/cancel-job/${jobId}`, { headers })
      .pipe(catchError(this.errorService.handleError));
  }

  completeJob(jobId: string): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    return this.http.post(`/api/admin/import/complete-job/${jobId}`, { headers })
      .pipe(catchError(this.errorService.handleError));
  }
}
