import { Action } from '@ngrx/store';
import { ApplicationData, StaticData } from '../services/data.service';
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

export type AppActions = ClearErrorAction
    | LoadDataAction
    | LoadDataDoneAction
    | LoadErrorAction
    | UpdateDataAction
    | UpdateDataDoneAction
    | UpdateDataErrorAction
    | ChangeSelectedLanguage
    | LoadLocationDone;

