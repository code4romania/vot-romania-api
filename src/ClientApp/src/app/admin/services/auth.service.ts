import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ErrorService } from 'src/app/services/error.service';

export interface LoginCredentials {
    userName: string;
    password: string;
}

export interface LoginResponse {
    token: string;
}

@Injectable({providedIn: 'root'})
export class AuthService {
    constructor(private http: HttpClient,
                private errorService: ErrorService) {
    }

    public getToken(): string {
        return localStorage.getItem('token');
    }

    public login(credentials: LoginCredentials): Observable<LoginResponse> {
        return this.http.post<LoginResponse>(`/api/admin/login`, credentials)
            .pipe(catchError(this.errorService.handleError));
    }
}
