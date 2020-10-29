import { Action } from '@ngrx/store';
import { ProblemDetails, StaticData, ApplicationData, PaginatedResponse, PaginationDetails, AssignedAddress } from 'src/app/services/data.service';
import { LocationDetails } from 'src/app/services/here-address.service';
import { ImportedPollingStation, ImportJobDetails, ImportedPollingStationsFilter } from '../services/polling-stations.service';


const typeCache: { [label: string]: boolean } = {};

export function actionType<T extends string>(label: T): T {
    if (typeCache[label as string]) {
        throw new Error(`Action type "${label}" is not unqiue"`);
    }

    typeCache[label as string] = true;

    return label as T;
}

export function ShowLoader() {
    return function (Class: Function) {
        Object.defineProperty(Class.prototype, 'showLoader', {
            value: true
        });
    };
}

export function HideLoader() {
    return function (Class: Function) {
        Object.defineProperty(Class.prototype, 'showLoader', {
            value: false
        });
    };
}

export class AdminActionTypes {
    static readonly CLEAR_ERROR = actionType('Clear error');

    static readonly LOAD_DATA = actionType('Load data');
    static readonly LOAD_DATA_DONE = actionType('Load data done');
    static readonly LOAD_ERROR = actionType('Load error');

    static readonly UPDATE_DATA = actionType('Update data');
    static readonly UPDATE_DATA_DONE = actionType('Update data done');
    static readonly UPDATE_DATA_ERROR = actionType('Update data error');

    static readonly CHANGE_LANGUAGE = actionType('Change language');

    static readonly LOAD_LOCATIONS = actionType('Load locations');
    static readonly LOAD_LOCATIONS_DONE = actionType('Load locations done');
    static readonly LOAD_LOCATIONS_ERROR = actionType('Load locations error');

    static readonly LOAD_IPS = actionType('[ImportedPollingStations] Loading');
    static readonly LOAD_IPS_DONE = actionType('[ImportedPollingStations] Load success');
    static readonly LOAD_IPS_ERROR = actionType('[ImportedPollingStations] Load failure');

    static readonly LOAD_IMPORT_JOB_DETAILS = actionType('[ImportJobDetails] Loading');
    static readonly LOAD_IMPORT_JOB_DETAILS_DONE = actionType('[ImportDetails] Load success');
    static readonly LOAD_IMPORT_JOB_DETAILS_ERROR = actionType('[ImportDetails] Load failure');

    static readonly DELETE_IMPORTED_POLLING_STATION = actionType('[ImportedPollingStation] Delete');
    static readonly DELETE_IMPORTED_POLLING_STATION_DONE = actionType('[ImportedPollingStation] Delete success');
    static readonly DELETE_IMPORTED_POLLING_STATION_ERROR = actionType('[ImportedPollingStation] Delete failure');

    static readonly CREATE_IMPORTED_POLLING_STATION = actionType('[ImportedPollingStation] Create');
    static readonly CREATE_IMPORTED_POLLING_STATION_DONE = actionType('[ImportedPollingStation] Create success');
    static readonly CREATE_IMPORTED_POLLING_STATION_ERROR = actionType('[ImportedPollingStation] Create failure');

    static readonly UPDATE_IMPORTED_POLLING_STATION = actionType('[ImportedPollingStation] Update');
    static readonly UPDATE_IMPORTED_POLLING_STATION_DONE = actionType('[ImportedPollingStation] Update success');
    static readonly UPDATE_IMPORTED_POLLING_STATION_ERROR = actionType('[ImportedPollingStation] Update failure');

    static readonly RESTART_JOB = actionType('[ImportJob] Restart job');
    static readonly RESTART_JOB_DONE = actionType('[ImportJob] Restart job success');
    static readonly RESTART_JOB_ERROR = actionType('[ImportJob] Restart job failure');

    static readonly CANCEL_JOB = actionType('[ImportJob]  Cancel job');
    static readonly CANCEL_JOB_DONE = actionType('[ImportJob] Cancel job success');
    static readonly CANCEL_JOB_ERROR = actionType('[ImportJob] Cancel job failure');

    static readonly FINISH_JOB = actionType('[ImportJob] Finish job');
    static readonly FINISH_JOB_DONE = actionType('[ImportJob] Finish job success');
    static readonly FINISH_JOB_ERROR = actionType('[ImportJob] Finish job failure');

    static readonly UPDATE_PAGINATION = actionType('[ImportedPollingStation] Update pagination');
    static readonly UPDATE_FILTER = actionType('[ImportedPollingStation] Update filter');
    static readonly RESET_FILTER = actionType('[ImportedPollingStation] Reset filter');

    static readonly DISPLAY_TOASTER_MESSAGE = actionType('[ToasterService] Show message');
}

export interface FailedAction extends Action {
    error: ProblemDetails;
}

export interface SuccessAction extends Action {
    message: string;
}


export class ClearErrorAction implements Action {
    readonly type = AdminActionTypes.CLEAR_ERROR;
}

@ShowLoader()
export class LoadDataAction implements Action {
    readonly type = AdminActionTypes.LOAD_DATA;
}

@HideLoader()
export class LoadErrorAction implements FailedAction {
    readonly type = AdminActionTypes.LOAD_ERROR;
    constructor(public error: ProblemDetails) { }
}

@HideLoader()
export class LoadDataDoneAction implements Action {
    readonly type = AdminActionTypes.LOAD_DATA_DONE;
    payload: {
        data: ApplicationData
    };

    constructor(data: ApplicationData) {
        this.payload = {
            data
        };
    }
}

@ShowLoader()
export class UpdateDataAction implements Action {
    readonly type = AdminActionTypes.UPDATE_DATA;

    payload: {
        language: string,
        data: StaticData
    };

    constructor(data: StaticData, language: string) {
        this.payload = {
            language,
            data
        };
    }
}

@HideLoader()
export class UpdateDataErrorAction implements FailedAction {
    readonly type = AdminActionTypes.UPDATE_DATA_ERROR;
    constructor(public error: ProblemDetails) { }
}

@HideLoader()
export class UpdateDataDoneAction implements Action {
    readonly type = AdminActionTypes.UPDATE_DATA_DONE;

    constructor() {
    }
}

@ShowLoader()
export class ChangeSelectedLanguage implements Action {
    readonly type = AdminActionTypes.CHANGE_LANGUAGE;

    constructor(public payload: string) { }
}

@ShowLoader()
export class LoadLocations implements Action {
    readonly type = AdminActionTypes.LOAD_LOCATIONS;

    constructor(public locationId: string) { }
}

@HideLoader()
export class LoadLocationError implements FailedAction {
    readonly type = AdminActionTypes.LOAD_LOCATIONS_ERROR;
    constructor(public error: ProblemDetails) { }
}

@HideLoader()
export class LoadLocationDone implements Action {
    readonly type = AdminActionTypes.LOAD_LOCATIONS_DONE;
    constructor(public userLocation: LocationDetails, public pollingStations) { }
}

@ShowLoader()
export class LoadImportedPollingStationsAction implements Action {
    public readonly type = AdminActionTypes.LOAD_IPS;
    constructor() { }
}

@HideLoader()
export class LoadImportedPollingStationsSuccessAction implements Action {
    public readonly type = AdminActionTypes.LOAD_IPS_DONE;
    constructor(public payload: PaginatedResponse<ImportedPollingStation>) { }
}

@HideLoader()
export class LoadImportedPollingStationsFailAction implements FailedAction {
    public readonly type = AdminActionTypes.LOAD_IPS_ERROR;
    constructor(public error: ProblemDetails) { }
}

@ShowLoader()
export class LoadImportJobDetailsAction implements Action {
    public readonly type = AdminActionTypes.LOAD_IMPORT_JOB_DETAILS;
    constructor() { }
}

@HideLoader()
export class LoadImportJobDetailsSuccessAction implements Action {
    public readonly type = AdminActionTypes.LOAD_IMPORT_JOB_DETAILS_DONE;
    constructor(public importJobDetails: ImportJobDetails) { }
}

@HideLoader()
export class LoadImportJobDetailsFailAction implements FailedAction {
    public readonly type = AdminActionTypes.LOAD_IMPORT_JOB_DETAILS_ERROR;
    constructor(public error: ProblemDetails) { }
}

@ShowLoader()
export class DeleteImportedPollingStationAction implements Action {
    public readonly type = AdminActionTypes.DELETE_IMPORTED_POLLING_STATION;
    constructor(public jobId: string, public importedPollingStationId: number) { }
}

@HideLoader()
export class DeleteImportedPollingStationSuccessAction implements Action {
    public readonly type = AdminActionTypes.DELETE_IMPORTED_POLLING_STATION_DONE;
    constructor() { }
}

@HideLoader()
export class DeleteImportedPollingStationFailAction implements FailedAction {
    public readonly type = AdminActionTypes.DELETE_IMPORTED_POLLING_STATION_ERROR;
    constructor(public error: ProblemDetails) { }
}

export class UpdatePagination implements Action {
    public readonly type = AdminActionTypes.UPDATE_PAGINATION;
    constructor(public payload: PaginationDetails) { }
}

export class UpdateFilter implements Action {
    public readonly type = AdminActionTypes.UPDATE_FILTER;
    constructor(public payload: ImportedPollingStationsFilter) { }
}

export class ResetFilter implements Action {
    public readonly type = AdminActionTypes.RESET_FILTER;
    constructor() { }
}

export class DisplayToasterMessage implements Action {
    public readonly type = AdminActionTypes.DISPLAY_TOASTER_MESSAGE;
    constructor(public text: string, public severity: 'success' | 'warning') { }
}


@ShowLoader()
export class CreateImportedPollingStationAction implements Action {
    public readonly type = AdminActionTypes.CREATE_IMPORTED_POLLING_STATION;
    constructor(public jobId: string, public importedPollingStation: ImportedPollingStation, public adddresses: AssignedAddress[]) { }
}

@HideLoader()
export class CreateImportedPollingStationSuccessAction implements Action {
    public readonly type = AdminActionTypes.CREATE_IMPORTED_POLLING_STATION_DONE;
    constructor() { }
}

@HideLoader()
export class CreateImportedPollingStationFailAction implements FailedAction {
    public readonly type = AdminActionTypes.CREATE_IMPORTED_POLLING_STATION_ERROR;
    constructor(public error: ProblemDetails) { }
}


@ShowLoader()
export class UpdateImportedPollingStationAction implements Action {
    public readonly type = AdminActionTypes.UPDATE_IMPORTED_POLLING_STATION;
    constructor(
        public jobId: string,
        public pollingStationId: number,
        public importedPollingStation: ImportedPollingStation,
        public adddresses: AssignedAddress[]) { }
}

@HideLoader()
export class UpdateImportedPollingStationSuccessAction implements Action {
    public readonly type = AdminActionTypes.UPDATE_IMPORTED_POLLING_STATION_DONE;
    constructor() { }
}

@HideLoader()
export class UpdateImportedPollingStationFailAction implements FailedAction {
    public readonly type = AdminActionTypes.UPDATE_IMPORTED_POLLING_STATION_ERROR;
    constructor(public error: ProblemDetails) { }
}

@ShowLoader()
export class CancelImportJobAction implements Action {
    public readonly type = AdminActionTypes.CANCEL_JOB;
    constructor(public jobId: string) { }
}

@HideLoader()
export class CancelImportJobSuccessAction implements Action {
    public readonly type = AdminActionTypes.CANCEL_JOB_DONE;
    constructor() { }
}

@HideLoader()
export class CancelImportJobFailAction implements FailedAction {
    public readonly type = AdminActionTypes.CANCEL_JOB_ERROR;
    constructor(public error: ProblemDetails) { }
}


@ShowLoader()
export class FinishImportJobAction implements Action {
    public readonly type = AdminActionTypes.FINISH_JOB;
    constructor(public jobId: string) { }
}

@HideLoader()
export class FinishImportJobSuccessAction implements Action {
    public readonly type = AdminActionTypes.FINISH_JOB_DONE;
    constructor() { }
}

@HideLoader()
export class FinishImportJobFailAction implements FailedAction {
    public readonly type = AdminActionTypes.FINISH_JOB_ERROR;
    constructor(public error: ProblemDetails) { }
}

@ShowLoader()
export class RestartImportJobAction implements Action {
    public readonly type = AdminActionTypes.RESTART_JOB;
    constructor(public jobId: string) { }
}

@HideLoader()
export class RestartImportJobSuccessAction implements Action {
    public readonly type = AdminActionTypes.RESTART_JOB_DONE;
    constructor() { }
}

@HideLoader()
export class RestartImportJobFailAction implements FailedAction {
    public readonly type = AdminActionTypes.RESTART_JOB_ERROR;
    constructor(public error: ProblemDetails) { }
}

export type AdminActions = ClearErrorAction
    | LoadDataAction
    | LoadDataDoneAction
    | LoadErrorAction
    | UpdateDataAction
    | UpdateDataDoneAction
    | UpdateDataErrorAction
    | ChangeSelectedLanguage
    | LoadLocationDone
    | LoadImportedPollingStationsAction
    | LoadImportedPollingStationsSuccessAction
    | LoadImportedPollingStationsFailAction
    | LoadImportJobDetailsAction
    | LoadImportJobDetailsSuccessAction
    | LoadImportJobDetailsFailAction
    | DeleteImportedPollingStationAction
    | DeleteImportedPollingStationSuccessAction
    | DeleteImportedPollingStationFailAction
    | UpdateFilter
    | ResetFilter
    | UpdatePagination
    | DisplayToasterMessage
    | CreateImportedPollingStationAction
    | CreateImportedPollingStationFailAction
    | CreateImportedPollingStationSuccessAction
    | UpdateImportedPollingStationAction
    | UpdateImportedPollingStationFailAction
    | UpdateImportedPollingStationSuccessAction

    | CancelImportJobAction
    | CancelImportJobSuccessAction
    | CancelImportJobFailAction

    | RestartImportJobAction
    | RestartImportJobSuccessAction
    | RestartImportJobFailAction

    | FinishImportJobAction
    | FinishImportJobSuccessAction
    | FinishImportJobFailAction
    ;


