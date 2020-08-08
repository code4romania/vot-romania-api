import { AppActions, ActionTypes } from './actions';
import { VotingGuide, StaticData, PollingStationGroup, ImportedPollingStation, PaginatedResponse, ImportJobDetails, PaginationDetails, ImportedPollingStationsFilter as ImportedPollingStationsFilter } from '../services/data.service';
import { AuthActions, AuthActionTypes } from './auth';

import { LocationDetails } from '../services/here-address.service';
import { isEqual } from 'lodash';

export interface ImportPollingStationsState {
    importedPollingStationsPagination: PaginationDetails;
    importedPollingStationsFilter: ImportedPollingStationsFilter;
    errorMessage: string;
    hasError: boolean;
    importJobDetails: ImportJobDetails;
    isLoading: boolean;
    importedPollingStations: PaginatedResponse<ImportedPollingStation>;
}

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
    };
    import: ImportPollingStationsState
}

const DEFAULT_IPS_FILTER: ImportedPollingStationsFilter = {
    address: '',
    county: '',
    institution: '',
    locality: '',
    pollingStationNumber: '',
    status: ''
};

const DEFAULT_PAGINATION: PaginationDetails = {
    pageNumber: 0,
    pageSize: 5
};

const initialState: ApplicationState = {
    languages: [],
    votingGuide: undefined,
    staticTexts: undefined,
    error: '',
    generalInfo: '',
    selectedLanguage: 'Ro', // change to enum
    pollingStations: [],
    selectedAddressDetails: undefined,
    auth: { token: '' },
    import: {
        importJobDetails: undefined,
        importedPollingStations: undefined,
        errorMessage: '',
        hasError: false,
        isLoading: true,
        importedPollingStationsFilter: DEFAULT_IPS_FILTER,
        importedPollingStationsPagination: DEFAULT_PAGINATION
    }
};
export function appStateReducer(state: ApplicationState = initialState, action: AppActions | AuthActions): ApplicationState {
    switch (action.type) {
        case ActionTypes.LOAD_DATA_DONE:
            const languageData = action.payload.data.content.find(x => x.language === state.selectedLanguage);
            if (languageData === undefined) {
                return state;
            }

            return {
                ...state,
                languages: action.payload.data.content.map(x => x.language),
                staticTexts: action.payload.data.content,
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

        case ActionTypes.CLEAR_ERROR:
            return {
                ...state,
                error: '',
            };

        case ActionTypes.UPDATE_DATA_ERROR:
            return {
                ...state,
                error: action.payload
            };

        case AuthActionTypes.LOGIN_SUCCEEDED:
            const { token } = action.payload;
            return {
                ...state,
                error: '',
                auth: { token }
            };
        case AuthActionTypes.LOGIN_FAILED:
            return {
                ...state,
                error: action.payload,
                auth: { token: '' }
            };
        case AuthActionTypes.LOGOUT:
            return {
                ...state,
                error: '',
                auth: { token: '' }
            };

        case ActionTypes.LOAD_LOCATIONS_DONE:
            return {
                ...state,
                pollingStations: action.pollingStations,
                selectedAddressDetails: action.userLocation
            };

        case ActionTypes.LOAD_IMPORT_JOB_DETAILS_DONE:
            if (action.importJobDetails) {
                return {
                    ...state,
                    import: {
                        ...state.import,
                        importJobDetails: action.importJobDetails
                    },
                };
            } else {
                return state;
            }

        case ActionTypes.LOAD_IMPORT_JOB_DETAILS_ERROR:
            return {
                ...state,
                import: {
                    ...state.import,
                    importJobDetails: undefined,
                    errorMessage: action.error
                },
            };

        case ActionTypes.LOAD_IPS_DONE:
            return {
                ...state,
                import: {
                    ...state.import,
                    importedPollingStations: action.payload,
                    errorMessage: '',
                    isLoading: false,
                    hasError: false,
                },
            };
        case ActionTypes.LOAD_IPS_ERROR:
            return {
                ...state,
                import: {
                    ...state.import,
                    importedPollingStations: undefined,
                    errorMessage: action.error,
                    isLoading: false,
                    hasError: true,
                },
            };

        case ActionTypes.RESET_FILTER:
            return {
                ...state,
                import: {
                    ...state.import,
                    importedPollingStationsFilter: DEFAULT_IPS_FILTER
                }
            }

        case ActionTypes.UPDATE_FILTER:
            if (isEqual(state.import.importedPollingStationsFilter, action.payload)) {
                return state;
            }

            return {
                ...state,
                import: {
                    ...state.import,
                    importedPollingStationsFilter: action.payload
                }
            }

            case ActionTypes.UPDATE_PAGINATION:
                return {
                    ...state,
                    import: {
                        ...state.import,
                        importedPollingStationsPagination: action.payload
                    }
                }
        default:
            return state;
    }
}
