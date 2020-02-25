import { Action } from '@ngrx/store';
import { ApplicationData } from '../services/data.service';

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
    static readonly LOAD_DATA_ERROR = actionType('Load data error');
    static readonly CHANGE_LANGUAGE = actionType('Change language');
}

export class LoadDataAction implements Action {
    readonly type = ActionTypes.LOAD_DATA;
}
export class LoadDataErrorAction implements Action {
    readonly type = ActionTypes.LOAD_DATA_ERROR;
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

export class ChangeSelectedLanguage implements Action{
    readonly type = ActionTypes.CHANGE_LANGUAGE;

    constructor(public payload: string) { }
}

export type AppActions = LoadDataAction | LoadDataDoneAction | LoadDataErrorAction | ChangeSelectedLanguage;
