import { Injectable } from '@angular/core';
import { LoadDataDoneAction, LoadDataErrorAction, ActionTypes } from './actions';
import { Observable, of } from 'rxjs';
import { mergeMap, map, catchError } from 'rxjs/operators';
import { Action } from '@ngrx/store';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { DataService } from '../services/data.service';

@Injectable({ providedIn: 'root' })
export class ApplicationEffects {

    constructor(private dataService: DataService,
        private actions$: Actions) { }

    @Effect()
    loadData$: Observable<Action> = this.actions$.pipe(
        ofType(ActionTypes.LOAD_DATA),
        mergeMap(action =>
            this.dataService.getData().pipe(
                map(data => (new LoadDataDoneAction(data))),
                catchError(err => of(new LoadDataErrorAction(err)))
            )
        )
    );
}
