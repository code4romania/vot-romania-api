import { Injectable } from '@angular/core';
import {
    LoadDataDoneAction,
    LoadErrorAction,
    AdminActionTypes,
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
    UpdateImportedPollingStationAction,
    RestartImportJobAction,
    RestartImportJobSuccessAction,
    RestartImportJobFailAction,
    CancelImportJobFailAction,
    CancelImportJobSuccessAction,
    CancelImportJobAction,
    FinishImportJobAction,
    FinishImportJobSuccessAction,
    FailedAction
} from './admin-actions';
import { Observable, of, empty } from 'rxjs';
import { mergeMap, map, catchError, switchMap, tap, withLatestFrom, filter } from 'rxjs/operators';
import { Action, Store } from '@ngrx/store';
import { Actions, Effect, ofType } from '@ngrx/effects';

import { Router } from '@angular/router';
import { getCurrentImportedPollingStationsFilter, getCurrentImportJobDetails, getCurrentImportedPollingStationsPagination } from './admin-selectors';
import { PaginatedResponse } from 'src/app/services/data.service';
import { HereAddressService } from 'src/app/services/here-address.service';
import { ToasterService } from '../services/toaster.service';
import { ApplicationState } from 'src/app/state/reducers';
import { ImportedPollingStation, PollingStationsService } from '../services/polling-stations.service';
import { SpinnerService } from '../services/spinner.service';

@Injectable({ providedIn: 'root' })
export class AdminEffects {

    constructor(private dataService: PollingStationsService,
        private addressService: HereAddressService,
        private router: Router,
        private actions$: Actions,
        private toasterService: ToasterService,
        private spinnerService: SpinnerService,
        private store$: Store<ApplicationState>) {
    }

    @Effect()
    loadData$: Observable<Action> = this.actions$.pipe(
        ofType(AdminActionTypes.LOAD_DATA, AdminActionTypes.UPDATE_DATA_DONE),
        mergeMap(() =>
            this.dataService.getData().pipe(
                map(data => (new LoadDataDoneAction(data))),
                catchError(err => of(new LoadErrorAction(err)))
            )
        )
    );

    @Effect()
    updateData$: Observable<Action> = this.actions$.pipe(
        ofType(AdminActionTypes.UPDATE_DATA),
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
        ofType(AdminActionTypes.UPDATE_DATA_DONE),
        tap(() => {
            this.router.navigate(['admin']);
        })
    );

    @Effect()
    getLocations$: Observable<Action> = this.actions$.pipe(
        ofType<LoadLocations>(AdminActionTypes.LOAD_LOCATIONS),

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
        ofType<LoadImportedPollingStationsAction>(AdminActionTypes.LOAD_IPS),
        withLatestFrom(
            this.store$.select(getCurrentImportJobDetails),
            this.store$.select(getCurrentImportedPollingStationsFilter),
            this.store$.select(getCurrentImportedPollingStationsPagination)
        ),
        filter(([, jobDetails,]) => jobDetails ? true : false),
        switchMap(([, jobDetails, filter, pagination]) =>
            this.dataService.getImportedPollingStations(jobDetails.jobId, filter, pagination).pipe(
                map((response: PaginatedResponse<ImportedPollingStation>) => new LoadImportedPollingStationsSuccessAction(response)),
                catchError((error) => of(new LoadImportedPollingStationsFailAction(error)))
            )
        )
    );

    @Effect()
    loadImportJobDetails$: Observable<Action> = this.actions$.pipe(
        ofType<LoadImportJobDetailsAction>(AdminActionTypes.LOAD_IMPORT_JOB_DETAILS),
        mergeMap(() =>
            this.dataService.getImportJobDetails().pipe(
                switchMap((data) => [new LoadImportJobDetailsSuccessAction(data), new LoadImportedPollingStationsAction()]),
                catchError(err => of(new LoadImportJobDetailsFailAction(err)))
            )
        )
    );

    @Effect()
    deleteImportedPollingStation$: Observable<Action> = this.actions$.pipe(
        ofType<DeleteImportedPollingStationAction>(AdminActionTypes.DELETE_IMPORTED_POLLING_STATION),
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
        ofType(AdminActionTypes.RESET_FILTER, AdminActionTypes.UPDATE_FILTER, AdminActionTypes.UPDATE_PAGINATION),
        switchMap(() => of(new LoadImportedPollingStationsAction()))
    );

    @Effect()
    createImportedPollingStation$: Observable<Action> = this.actions$.pipe(
        ofType<CreateImportedPollingStationAction>(AdminActionTypes.CREATE_IMPORTED_POLLING_STATION),
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
        ofType<UpdateImportedPollingStationAction>(AdminActionTypes.UPDATE_IMPORTED_POLLING_STATION),
        map((action) => ({ jobId: action.jobId, pollingStaionId: action.pollingStationId, importedPollingStation: action.importedPollingStation, addresses: action.adddresses })),
        mergeMap(({ jobId, pollingStaionId, importedPollingStation, addresses }) =>
            this.dataService.updateImportedPollingStation(jobId, pollingStaionId, importedPollingStation, addresses).pipe(
                switchMap(() => [new CreateImportedPollingStationSuccessAction(), new LoadImportedPollingStationsAction(), new DisplayToasterMessage("Create successfull", 'success')]),
                catchError(err => of(new CreateImportedPollingStationFailAction(err)))
            )
        )
    );

    @Effect()
    restartCurrentJob$: Observable<Action> = this.actions$.pipe(
        ofType<RestartImportJobAction>(AdminActionTypes.RESTART_JOB),
        map((action) => ({ jobId: action.jobId })),
        mergeMap(({ jobId }) =>
            this.dataService.restartJob(jobId).pipe(
                switchMap(() => [new RestartImportJobSuccessAction(), new LoadImportJobDetailsAction(), new DisplayToasterMessage("Restart successfull", 'success')]),
                catchError(err => of(new RestartImportJobFailAction(err)))
            )
        )
    );

    @Effect()
    cancelCurrentJob$: Observable<Action> = this.actions$.pipe(
        ofType<CancelImportJobAction>(AdminActionTypes.CANCEL_JOB),
        map((action) => ({ jobId: action.jobId })),
        mergeMap(({ jobId }) =>
            this.dataService.cancelJob(jobId).pipe(
                switchMap(() => [new CancelImportJobSuccessAction(), new LoadImportJobDetailsAction(), new DisplayToasterMessage("Cancel successfull", 'success')]),
                catchError(err => of(new CancelImportJobFailAction(err)))
            )
        )
    );

    @Effect()
    finishCurrentJob$: Observable<Action> = this.actions$.pipe(
        ofType<FinishImportJobAction>(AdminActionTypes.FINISH_JOB),
        map((action) => ({ jobId: action.jobId })),
        mergeMap(({ jobId }) =>
            this.dataService.completeJob(jobId).pipe(
                switchMap(() => [new FinishImportJobSuccessAction(), new LoadImportJobDetailsAction(), new DisplayToasterMessage("Finish successfull", 'success')]),
                catchError(err => of(new CancelImportJobFailAction(err)))
            )
        )
    );

    @Effect({ dispatch: false })
    displayToasterMessage$: Observable<Action> = this.actions$.pipe(
        ofType<DisplayToasterMessage>(AdminActionTypes.DISPLAY_TOASTER_MESSAGE),
        tap((action: DisplayToasterMessage) => this.toasterService.show(action.text, action.severity))
    );

    @Effect({ dispatch: false })
    displayErrorToasterMessage$: Observable<FailedAction> = this.actions$.pipe(
        ofType<FailedAction>(
            AdminActionTypes.LOAD_IPS_ERROR,
            AdminActionTypes.LOAD_IMPORT_JOB_DETAILS_ERROR,
            AdminActionTypes.DELETE_IMPORTED_POLLING_STATION_ERROR,
            AdminActionTypes.CREATE_IMPORTED_POLLING_STATION_ERROR ,
            AdminActionTypes.UPDATE_IMPORTED_POLLING_STATION_ERROR,
            AdminActionTypes.RESTART_JOB_ERROR,
            AdminActionTypes.CANCEL_JOB_ERROR,
            AdminActionTypes.FINISH_JOB_ERROR
        ),
        tap(details => {this.toasterService.show(details.error.detail , 'warning'); return empty();})
    );

    @Effect({dispatch: false})
    showLoader$ = this.actions$.pipe(
      filter((action: any) => action && action.showLoader !== undefined),
      map((action: any) => this.spinnerService.display(action.showLoader))
    );
}

