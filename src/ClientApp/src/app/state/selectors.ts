import { createSelector, createFeatureSelector } from '@ngrx/store';
import { ApplicationState } from './reducers';
import { get } from 'lodash';

const getApplicationState = createFeatureSelector<ApplicationState>('data');

export const getLanguages = createSelector(
    getApplicationState,
    state => state.languages
);
export const getSelectedLanguage = createSelector(
    getApplicationState,
    state => state.selectedLanguage
);

export const getGeneralInfo = createSelector(
    getApplicationState,
    state => state.generalInfo
);

export const getVotingGuide = createSelector(
    getApplicationState,
    state => state.votingGuide
);


export const getSelectedAddressDetails = createSelector(
    getApplicationState,
    state => state.selectedAddressDetails
);

export const getMapPins = createSelector(
    getApplicationState,
    state => ({ userAddress: state.selectedAddressDetails, pollingStations: state.pollingStations })
);


export const getError = createSelector(
    getApplicationState,
    state => state.error
);

export const getAuthToken = createSelector(
    getApplicationState,
    state => state.auth.token
);

export const getImportedPollingStations = createSelector(
    getApplicationState,
    state => get(state.import.importedPollingStations, 'results', [])
);


export const getImportedPollingStationsTotal = createSelector(
    getApplicationState,
    state => get(state.import.importedPollingStations, 'rowCount')
);

export const getCurrentImportJobDetails = createSelector(
    getApplicationState,
    state => state.import.importJobDetails
);
export const getCurrentImportedPollingStationsFilter = createSelector(
    getApplicationState,
    state => state.import.importedPollingStationsFilter
);
export const getCurrentImportedPollingStationsPagination = createSelector(
    getApplicationState,
    state => state.import.importedPollingStationsPagination
);
