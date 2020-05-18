import { AppActions, ActionTypes } from './actions';
import { VotingGuide, StaticData, PollingStationGroup } from '../services/data.service';
import { AuthActions, AuthActionTypes } from './auth';

import { LocationDetails } from '../services/here-address.service';
import { get } from 'lodash';
export interface ApplicationState {
    languages: string[];
    generalInfo: string;
    selectedLanguage: string;
    votingGuide: VotingGuide;
    staticTexts: StaticData[];
    error: string;
    pollingStations: PollingStationGroup[];
    selectedAddressDetails: LocationDetails;
    auth: {
        token: string;
        error: string;
    };
}

const initialState: ApplicationState = {
    languages: [],
    votingGuide: undefined,
    staticTexts: undefined,
    error: '',
    generalInfo: '',
    selectedLanguage: 'Ro', // change to enum
    pollingStations: [],
    selectedAddressDetails: undefined,
    auth: { token: '', error: '' },
};
export function appStateReducer(state: ApplicationState = initialState, action: AppActions | AuthActions): ApplicationState {
    switch (action.type) {
        case ActionTypes.LOAD_DATA_DONE:
            const languageData = action.payload.data.staticTexts.find(x => x.language === state.selectedLanguage);
            if (languageData === undefined) {
                return state;
            }

            return {
                ...state,
                languages: action.payload.data.staticTexts.map(x => x.language),
                staticTexts: action.payload.data.staticTexts,
                generalInfo: languageData.generalInfo,
                votingGuide: languageData.votersGuide
            };
        case ActionTypes.CHANGE_LANGUAGE:
            const changedLanguageData = state.staticTexts.find(x => x.language === action.payload);

            if (changedLanguageData === undefined) {
                return state;
            }

            return {
                ...state,
                generalInfo: changedLanguageData.generalInfo,
                votingGuide: changedLanguageData.votersGuide,
                selectedLanguage: action.payload
            };
        case AuthActionTypes.LOGIN_SUCCEEDED:
            const { authToken } = action.payload;
            return {
                ...state,
                auth: { token: authToken, error: '' }
            };
        case AuthActionTypes.LOGIN_FAILED:
            return {
                ...state,
                auth: { token: '', error: action.payload }
            };
        case AuthActionTypes.LOGOUT:
            return {
                ...state,
                auth: { token: '', error: '' }
            };

        case ActionTypes.LOAD_LOCATIONS_DONE:
            return {
                ...state,
                pollingStations: action.pollingStations,
                selectedAddressDetails: action.userLocation
            };

        default:
            return state;
    }
}
