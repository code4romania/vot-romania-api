import { Injectable } from '@angular/core';
import {
    LoadDataDoneAction,
    LoadErrorAction,
    ActionTypes,
    LoadLocationDone,
    LoadLocationError,
    LoadLocations,
    UpdateDataAction,
    UpdateDataDoneAction,
    UpdateDataErrorAction
} from './actions';
import { Observable, of } from 'rxjs';
import { mergeMap, map, catchError, switchMap, tap } from 'rxjs/operators';
import { Action } from '@ngrx/store';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { DataService } from '../services/data.service';
import { HereAddressService } from '../services/here-address.service';

import { Router } from '@angular/router';

@Injectable({providedIn: 'root'})
export class ApplicationEffects {

    constructor(private dataService: DataService,
                private addressService: HereAddressService,
                private router: Router,
                private actions$: Actions) {
    }

    @Effect()
    loadData$: Observable<Action> = this.actions$.pipe(
        ofType(ActionTypes.LOAD_DATA, ActionTypes.UPDATE_DATA_DONE),
        mergeMap(() =>
            this.dataService.getData().pipe(
                map(data => (new LoadDataDoneAction(data))),
                catchError(err => of(new LoadErrorAction(err)))
            )
        )
    );

    @Effect()
    updateData$: Observable<Action> = this.actions$.pipe(
        ofType(ActionTypes.UPDATE_DATA),
        mergeMap((action: UpdateDataAction) =>
            this.dataService.updateData(action.payload.data).pipe(
                map(() => (new UpdateDataDoneAction())),
                catchError(err => of(new UpdateDataErrorAction(err))
                )
            )
        )
    );

    @Effect({dispatch: false})
    updateDataSuccessful$: Observable<any> = this.actions$.pipe(
        ofType(ActionTypes.UPDATE_DATA_DONE),
        tap(() => {
            this.router.navigate(['admin']);
        })
    );

    @Effect()
    getUserTypeUserPermissions$: Observable<Action> = this.actions$.pipe(
        ofType<LoadLocations>(ActionTypes.LOAD_LOCATIONS),

        switchMap((action) => this.addressService.getLocationDetails(action.locationId)),
        switchMap((userLocation) => {
            const {displayPosition} = userLocation.response.view[0].result[0].location;
            return this.dataService.getPollingStations(displayPosition.latitude, displayPosition.longitude).pipe(map(result => {
                return {
                    userLocation: userLocation.response.view[0].result[0].location,
                    pollingStations: result
                };
            }));
        }),
        map((res) => {
            return new LoadLocationDone(res.userLocation, res.pollingStations);
        }),
        catchError(error => of(new LoadLocationError(error)))
    );
}
