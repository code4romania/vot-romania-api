import { Action } from '@ngrx/store';
import { ApplicationData, ProblemDetails } from '../services/data.service';
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
    static readonly LOAD_DATA = actionType('Load data');
    static readonly LOAD_DATA_DONE = actionType('Load data done');
    static readonly LOAD_ERROR = actionType('Load error');

    static readonly CHANGE_LANGUAGE = actionType('Change language');

    static readonly LOAD_LOCATIONS = actionType('Load locations');
    static readonly LOAD_LOCATIONS_DONE = actionType('Load locations done');
    static readonly LOAD_LOCATIONS_ERROR = actionType('Load locations error');
}

export interface FailedAction extends Action{
    error: ProblemDetails;
}

export interface SuccessAction extends Action{
    message: string;
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


export type AppActions = 
      LoadDataAction
    | LoadDataDoneAction
    | LoadErrorAction
    | ChangeSelectedLanguage
    | LoadLocationDone;


