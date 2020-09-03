import { createSelector, createFeatureSelector } from '@ngrx/store';
import { ApplicationState } from './reducers';

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
