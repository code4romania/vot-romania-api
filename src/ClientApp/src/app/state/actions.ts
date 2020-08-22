import { Action } from '@ngrx/store';
import { ApplicationData, StaticData, ImportedPollingStation, PaginatedResponse, ImportedPollingStationsFilter, PaginationDetails, ImportJobDetails, AssignedAddress, ProblemDetails } from '../services/data.service';
import { LocationDetails } from '../services/here-address.service';

const typeCache: { [label: string]: boolean } = {};

export function actionType<T extends string>(label: T): T {
    if (typeCache[label as string]) {
        throw new Error(`Action type "${label}" is not unqiue"`);
    }

    typeCache[label as string] = true;

    return label as T;
}

export class ActionTypes {
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

export interface FailedAction extends Action{
    error: ProblemDetails;
}

export interface SuccessAction extends Action{
    message: string;
}


export class ClearErrorAction implements Action {
    readonly type = ActionTypes.CLEAR_ERROR;
}

export class LoadDataAction implements Action {
    readonly type = ActionTypes.LOAD_DATA;
}

export class LoadErrorAction implements FailedAction {
    readonly type = ActionTypes.LOAD_ERROR;
    constructor(public error: ProblemDetails) { }    ;
}

export class LoadDataDoneAction implements Action {
    readonly type = ActionTypes.LOAD_DATA_DONE;
    payload: {
        data: ApplicationData
    };

    constructor(data: ApplicationData) {
        this.payload = {
            data
        };
    }
}

export class UpdateDataAction implements Action {
    readonly type = ActionTypes.UPDATE_DATA;

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

export class UpdateDataErrorAction implements FailedAction {
    readonly type = ActionTypes.UPDATE_DATA_ERROR;
    constructor(public error: ProblemDetails) {}
}

export class UpdateDataDoneAction implements Action {
    readonly type = ActionTypes.UPDATE_DATA_DONE;

    constructor() {
    }
}

export class ChangeSelectedLanguage implements Action {
    readonly type = ActionTypes.CHANGE_LANGUAGE;

    constructor(public payload: string) { }
}

export class LoadLocations implements Action {
    readonly type = ActionTypes.LOAD_LOCATIONS;

    constructor(public locationId: string) { }
}

export class LoadLocationError implements FailedAction {
    readonly type = ActionTypes.LOAD_LOCATIONS_ERROR;
    constructor(public error: ProblemDetails) { }
}

export class LoadLocationDone implements Action {
    readonly type = ActionTypes.LOAD_LOCATIONS_DONE;
    constructor(public userLocation: LocationDetails, public pollingStations) { }
}

export class LoadImportedPollingStationsAction implements Action {
    public readonly type = ActionTypes.LOAD_IPS;
    constructor() { }
}

export class LoadImportedPollingStationsSuccessAction implements Action {
    public readonly type = ActionTypes.LOAD_IPS_DONE;
    constructor(public payload: PaginatedResponse<ImportedPollingStation>) { }
}

export class LoadImportedPollingStationsFailAction implements FailedAction {
    public readonly type = ActionTypes.LOAD_IPS_ERROR;
    constructor(public error: ProblemDetails) { }
}

export class LoadImportJobDetailsAction implements Action {
    public readonly type = ActionTypes.LOAD_IMPORT_JOB_DETAILS;
    constructor() { }
}

export class LoadImportJobDetailsSuccessAction implements Action {
    public readonly type = ActionTypes.LOAD_IMPORT_JOB_DETAILS_DONE;
    constructor(public importJobDetails: ImportJobDetails) { }
}

export class LoadImportJobDetailsFailAction implements FailedAction {
    public readonly type = ActionTypes.LOAD_IMPORT_JOB_DETAILS_ERROR;
    constructor(public error: ProblemDetails) { }
}

export class DeleteImportedPollingStationAction implements Action {
    public readonly type = ActionTypes.DELETE_IMPORTED_POLLING_STATION;
    constructor(public jobId: string, public importedPollingStationId: number) { }
}

export class DeleteImportedPollingStationSuccessAction implements Action {
    public readonly type = ActionTypes.DELETE_IMPORTED_POLLING_STATION_DONE;
    constructor() { }
}

export class DeleteImportedPollingStationFailAction implements FailedAction {
    public readonly type = ActionTypes.DELETE_IMPORTED_POLLING_STATION_ERROR;
    constructor(public error: ProblemDetails) { }
}


export class UpdatePagination implements Action {
    public readonly type = ActionTypes.UPDATE_PAGINATION;
    constructor(public payload: PaginationDetails) { }
}

export class UpdateFilter implements Action {
    public readonly type = ActionTypes.UPDATE_FILTER;
    constructor(public payload: ImportedPollingStationsFilter) { }
}

export class ResetFilter implements Action {
    public readonly type = ActionTypes.RESET_FILTER;
    constructor() { }
}

export class DisplayToasterMessage implements Action {
    public readonly type = ActionTypes.DISPLAY_TOASTER_MESSAGE;
    constructor(public text: string, public severity: 'success' | 'warning') { }
}


export class CreateImportedPollingStationAction implements Action {
    public readonly type = ActionTypes.CREATE_IMPORTED_POLLING_STATION;
    constructor(public jobId: string, public importedPollingStation: ImportedPollingStation, public adddresses: AssignedAddress[]) { }
}

export class CreateImportedPollingStationSuccessAction implements Action {
    public readonly type = ActionTypes.CREATE_IMPORTED_POLLING_STATION_DONE;
    constructor() { }
}

export class CreateImportedPollingStationFailAction implements FailedAction {
    public readonly type = ActionTypes.CREATE_IMPORTED_POLLING_STATION_ERROR;
    constructor(public error: ProblemDetails) { }
}


export class UpdateImportedPollingStationAction implements Action {
    public readonly type = ActionTypes.UPDATE_IMPORTED_POLLING_STATION;
    constructor(public jobId: string, public pollingStationId: number, public importedPollingStation: ImportedPollingStation, public adddresses: AssignedAddress[]) { }
}

export class UpdateImportedPollingStationSuccessAction implements Action {
    public readonly type = ActionTypes.UPDATE_IMPORTED_POLLING_STATION_DONE;
    constructor() { }
}

export class UpdateImportedPollingStationFailAction implements FailedAction {
    public readonly type = ActionTypes.UPDATE_IMPORTED_POLLING_STATION_ERROR;
    constructor(public error: ProblemDetails) { }
}

export class CancelImportJobAction implements Action {
    public readonly type = ActionTypes.CANCEL_JOB;
    constructor(public jobId: string) { }
}

export class CancelImportJobSuccessAction implements Action {
    public readonly type = ActionTypes.CANCEL_JOB_DONE;
    constructor() { }
}

export class CancelImportJobFailAction implements FailedAction {
    public readonly type = ActionTypes.CANCEL_JOB_ERROR;
    constructor(public error: ProblemDetails) { }
}


export class FinishImportJobAction implements Action {
    public readonly type = ActionTypes.FINISH_JOB;
    constructor(public jobId: string) { }
}

export class FinishImportJobSuccessAction implements Action {
    public readonly type = ActionTypes.FINISH_JOB_DONE;
    constructor() { }
}

export class FinishImportJobFailAction implements FailedAction {
    public readonly type = ActionTypes.FINISH_JOB_ERROR;
    constructor(public error: ProblemDetails) { }
}

export class RestartImportJobAction implements Action {
    public readonly type = ActionTypes.RESTART_JOB;
    constructor(public jobId: string) { }
}

export class RestartImportJobSuccessAction implements Action {
    public readonly type = ActionTypes.RESTART_JOB_DONE;
    constructor() { }
}

export class RestartImportJobFailAction implements FailedAction {
    public readonly type = ActionTypes.RESTART_JOB_ERROR;
    constructor(public error: ProblemDetails) { }
}

export type AppActions = ClearErrorAction
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


