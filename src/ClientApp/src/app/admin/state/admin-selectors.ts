import { createSelector, createFeatureSelector } from '@ngrx/store';
import { get } from 'lodash';
import { AdminState } from './admin-reducers';

const getAdminState = createFeatureSelector<AdminState>('admin');

export const getLanguages = createSelector(
    getAdminState,
    state => state.languages
);
export const getSelectedLanguage = createSelector(
    getAdminState,
    state => state.selectedLanguage
);

export const getGeneralInfo = createSelector(
    getAdminState,
    state => state.generalInfo
);

export const getVotingGuide = createSelector(
    getAdminState,
    state => state.votingGuide
);

export const getError = createSelector(
    getAdminState,
    state => state.error
);

export const getAuthToken = createSelector(
    getAdminState,
    state => state.auth.token
);

export const getImportedPollingStations = createSelector(
    getAdminState,
    state => get(state.import.importedPollingStations, 'results', [])
);


export const getImportedPollingStationsTotal = createSelector(
    getAdminState,
    state => get(state.import.importedPollingStations, 'rowCount')
);

export const getCurrentImportJobDetails = createSelector(
    getAdminState,
    state => state.import.importJobDetails
);
export const getCurrentImportedPollingStationsFilter = createSelector(
    getAdminState,
    state =>  state.import.importedPollingStationsFilter
);
export const getCurrentImportedPollingStationsPagination = createSelector(
    getAdminState,
    state => state.import.importedPollingStationsPagination
);
