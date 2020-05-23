import { Injectable } from '@angular/core';
import { get } from 'lodash';
import { throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ErrorService {
  constructor() { }

  public handleError(err) {
    // in a real world app, we may send the server to some remote logging infrastructure
    // instead of just logging it to the console
    let errorMessage: string;
    if (err.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      errorMessage = `An error occurred: ${err.error.message}`;
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      errorMessage = `Backend returned code ${err.status}: ${get(err, 'body.error') || get(err, 'error.detail') || get(err, 'message')}`;
    }
    console.error(err);
    return throwError(errorMessage);
  }
}
