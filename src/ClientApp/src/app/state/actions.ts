import { Action } from '@ngrx/store';
import { ApplicationData, StaticData, ImportedPollingStation, PaginatedResponse, ImportedPollingStationsFilter, PaginationDetails, ImportJobDetails } from '../services/data.service';
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

    static readonly UPDATE_PAGINATION = actionType('[ImportedPollingStation] Update pagination');
    static readonly UPDATE_FILTER = actionType('[ImportedPollingStation] Update filter');
    static readonly RESET_FILTER = actionType('[ImportedPollingStation] Reset filter');

    static readonly DISPLAY_TOASTER_MESSAGE = actionType('[ToasterService] Show message');
}

export class ClearErrorAction implements Action {
    readonly type = ActionTypes.CLEAR_ERROR;
}

export class LoadDataAction implements Action {
    readonly type = ActionTypes.LOAD_DATA;
}

export class LoadErrorAction implements Action {
    readonly type = ActionTypes.LOAD_ERROR;
    constructor(public payload: string) { }
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

export class UpdateDataErrorAction implements Action {
    readonly type = ActionTypes.UPDATE_DATA_ERROR;
    constructor(public payload: string) {
        this.payload = payload;
    }
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

export class LoadLocationError implements Action {
    readonly type = ActionTypes.LOAD_LOCATIONS_ERROR;
    constructor(public error: any) { }
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

export class LoadImportedPollingStationsFailAction implements Action {
    public readonly type = ActionTypes.LOAD_IPS_ERROR;
    constructor(public error: any) { }
}

export class LoadImportJobDetailsAction implements Action {
    public readonly type = ActionTypes.LOAD_IMPORT_JOB_DETAILS;
    constructor() { }
}

export class LoadImportJobDetailsSuccessAction implements Action {
    public readonly type = ActionTypes.LOAD_IMPORT_JOB_DETAILS_DONE;
    constructor(public importJobDetails: ImportJobDetails) { }
}

export class LoadImportJobDetailsFailAction implements Action {
    public readonly type = ActionTypes.LOAD_IMPORT_JOB_DETAILS_ERROR;
    constructor(public error: any) { }
}

export class DeleteImportedPollingStationAction implements Action {
    public readonly type = ActionTypes.DELETE_IMPORTED_POLLING_STATION;
    constructor(public jobId: string, public importedPollingStationId: number) { }
}

export class DeleteImportedPollingStationSuccessAction implements Action {
    public readonly type = ActionTypes.DELETE_IMPORTED_POLLING_STATION_DONE;
    constructor() { }
}

export class DeleteImportedPollingStationFailAction implements Action {
    public readonly type = ActionTypes.DELETE_IMPORTED_POLLING_STATION_ERROR;
    constructor(public error: any) { }
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
    constructor(public text: string, public severity: 'success' | 'warning' | 'error') { }
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
    | DisplayToasterMessage;


