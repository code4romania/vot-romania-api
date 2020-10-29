import { Injectable } from '@angular/core';
import { throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ErrorService {
  constructor() { }

  public handleError(err) {
    console.error(err.error);
    return throwError(err.error);
  }
}
