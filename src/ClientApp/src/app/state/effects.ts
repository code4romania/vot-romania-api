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
    UpdateDataErrorAction,
    LoadImportedPollingStationsAction,
    LoadImportedPollingStationsSuccessAction,
    LoadImportedPollingStationsFailAction,
    LoadImportJobDetailsAction,
    LoadImportJobDetailsSuccessAction,
    LoadImportJobDetailsFailAction,
    DeleteImportedPollingStationAction,
    DeleteImportedPollingStationFailAction,
    DeleteImportedPollingStationSuccessAction,
    DisplayToasterMessage,
    CreateImportedPollingStationAction,
    CreateImportedPollingStationSuccessAction,
    CreateImportedPollingStationFailAction,
    UpdateImportedPollingStationAction
} from './actions';
import { Observable, of } from 'rxjs';
import { mergeMap, map, catchError, switchMap, tap, withLatestFrom, filter, distinctUntilChanged } from 'rxjs/operators';
import { Action, Store } from '@ngrx/store';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { DataService, PaginatedResponse, ImportedPollingStation, ImportJobDetails, ImportedPollingStationsFilter, PaginationDetails } from '../services/data.service';
import { HereAddressService } from '../services/here-address.service';

import { Router } from '@angular/router';
import { ApplicationState } from './reducers';
import { getCurrentImportedPollingStationsFilter, getCurrentImportJobDetails, getCurrentImportedPollingStationsPagination } from './selectors';
import { isEqual } from 'lodash';
import { ToasterService } from '../services/toaster.service';

@Injectable({ providedIn: 'root' })
export class ApplicationEffects {

    constructor(private dataService: DataService,
        private addressService: HereAddressService,
        private router: Router,
        private actions$: Actions,
        private toasterService: ToasterService,
        private store$: Store<ApplicationState>) {
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

    @Effect({ dispatch: false })
    updateDataSuccessful$: Observable<any> = this.actions$.pipe(
        ofType(ActionTypes.UPDATE_DATA_DONE),
        tap(() => {
            this.router.navigate(['admin']);
        })
    );

    @Effect()
    getLocations$: Observable<Action> = this.actions$.pipe(
        ofType<LoadLocations>(ActionTypes.LOAD_LOCATIONS),

        switchMap((action) => this.addressService.getLocationDetails(action.locationId)),
        switchMap((userLocation) => {
            const { displayPosition } = userLocation.response.view[0].result[0].location;
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

    @Effect()
    public loadImportedPollingStations$ = this.actions$.pipe(
        ofType<LoadImportedPollingStationsAction>(ActionTypes.LOAD_IPS),
        withLatestFrom(
            this.store$.select(getCurrentImportJobDetails),
            this.store$.select(getCurrentImportedPollingStationsFilter),
            this.store$.select(getCurrentImportedPollingStationsPagination)
        ),
        filter(([, jobDetails,]) => jobDetails !== undefined && jobDetails.jobId !== ''),
        switchMap(([, jobDetails, filter, pagination]) =>
            this.dataService.getImportedPollingStations(jobDetails.jobId, filter, pagination).pipe(
                map((response: PaginatedResponse<ImportedPollingStation>) => new LoadImportedPollingStationsSuccessAction(response)),
                catchError((error) => of(new LoadImportedPollingStationsFailAction(error)))
            )
        )
    );

    @Effect()
    loadImportJobDetails$: Observable<Action> = this.actions$.pipe(
        ofType<LoadImportJobDetailsAction>(ActionTypes.LOAD_IMPORT_JOB_DETAILS),
        mergeMap(() =>
            this.dataService.getImportJobDetails().pipe(
                switchMap((data) => [new LoadImportJobDetailsSuccessAction(data), new LoadImportedPollingStationsAction()]),
                catchError(err => of(new LoadImportJobDetailsFailAction(err)))
            )
        )
    );

    @Effect()
    deleteImportedPollingStation$: Observable<Action> = this.actions$.pipe(
        ofType<DeleteImportedPollingStationAction>(ActionTypes.DELETE_IMPORTED_POLLING_STATION),
        map((action) => ({ jobId: action.jobId, importedPollingStationId: action.importedPollingStationId })),
        mergeMap(({ jobId, importedPollingStationId }) =>
            this.dataService.deleteImportedPollingStationId(jobId, importedPollingStationId).pipe(
                switchMap(() => [new DeleteImportedPollingStationSuccessAction(), new LoadImportedPollingStationsAction(), new DisplayToasterMessage("Delete successfull", 'success')]),
                catchError(err => of(new DeleteImportedPollingStationFailAction(err)))
            )
        )
    );

    @Effect()
    fetchImportedPollingStations$: Observable<Action> = this.actions$.pipe(
        ofType(ActionTypes.RESET_FILTER, ActionTypes.UPDATE_FILTER, ActionTypes.UPDATE_PAGINATION),
        switchMap(() => of(new LoadImportedPollingStationsAction()))
    );

    @Effect()
    createImportedPollingStation$: Observable<Action> = this.actions$.pipe(
        ofType<CreateImportedPollingStationAction>(ActionTypes.CREATE_IMPORTED_POLLING_STATION),
        map((action) => ({ jobId: action.jobId, importedPollingStation: action.importedPollingStation, addresses: action.adddresses })),
        mergeMap(({ jobId, importedPollingStation, addresses }) =>
            this.dataService.addImportedPollingStation(jobId, importedPollingStation, addresses).pipe(
                switchMap(() => [new CreateImportedPollingStationSuccessAction(), new LoadImportedPollingStationsAction(), new DisplayToasterMessage("Create successfull", 'success')]),
                catchError(err => of(new CreateImportedPollingStationFailAction(err)))
            )
        )
    );

    @Effect()
    updateImportedPollingStation$: Observable<Action> = this.actions$.pipe(
        ofType<UpdateImportedPollingStationAction>(ActionTypes.UPDATE_IMPORTED_POLLING_STATION),
        map((action) => ({ jobId: action.jobId, pollingStaionId: action.pollingStationId, importedPollingStation: action.importedPollingStation, addresses: action.adddresses })),
        mergeMap(({ jobId, pollingStaionId, importedPollingStation, addresses }) =>
            this.dataService.updateImportedPollingStation(jobId, pollingStaionId, importedPollingStation, addresses).pipe(
                switchMap(() => [new CreateImportedPollingStationSuccessAction(), new LoadImportedPollingStationsAction(), new DisplayToasterMessage("Update successfull", 'success')]),
                catchError(err => of(new CreateImportedPollingStationFailAction(err)))
            )
        )
    );

    @Effect({ dispatch: false })
    displayToasterMessage$: Observable<Action> = this.actions$.pipe(
        ofType<DisplayToasterMessage>(ActionTypes.DISPLAY_TOASTER_MESSAGE),
        tap((action: DisplayToasterMessage) => this.toasterService.show(action.text, action.severity))
    );
}
