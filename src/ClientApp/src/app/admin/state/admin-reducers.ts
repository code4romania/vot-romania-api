import { PaginatedResponse, PaginationDetails, StaticData, VotingGuide } from 'src/app/services/data.service';

import {
    ImportedPollingStation,
    ImportedPollingStationsFilter,
    ImportJobDetails,
} from '../services/polling-stations.service';
import { AdminActions, AdminActionTypes } from './admin-actions';
import { AuthActions, AuthActionTypes } from './auth';

export interface ImportPollingStationsState {
    importedPollingStationsPagination: PaginationDetails;
    importedPollingStationsFilter: ImportedPollingStationsFilter;
    importJobDetails: ImportJobDetails;
    importedPollingStations: PaginatedResponse<ImportedPollingStation>;
}

export interface AdminState {
    isAdmin: boolean;
    languages: string[];
    generalInfo: string;
    selectedLanguage: string;
    votingGuide: VotingGuide;
    staticTexts: StaticData[];
    error: string;
    auth: {
        token: string;
    };
    import: ImportPollingStationsState;
}

const DEFAULT_IPS_FILTER: ImportedPollingStationsFilter = {
    address: '',
    county: '',
    institution: '',
    locality: '',
    pollingStationNumber: '',
    resolvedAddressStatus: ''
};

const DEFAULT_PAGINATION: PaginationDetails = {
    pageNumber: 1,
    pageSize: 5
};

const initialState: AdminState = {
    isAdmin: true,
    languages: [],
    votingGuide: undefined,
    staticTexts: undefined,
    error: '',
    generalInfo: '',
    selectedLanguage: 'Ro', // change to enum
    auth: { token: '' },
    import: {
        importJobDetails: undefined,
        importedPollingStations: undefined,
        importedPollingStationsFilter: DEFAULT_IPS_FILTER,
        importedPollingStationsPagination: DEFAULT_PAGINATION
    }
};
export function adminStateReducer(state: AdminState = initialState, action: AdminActions | AuthActions): AdminState {
    switch (action.type) {
        case AdminActionTypes.LOAD_DATA_DONE:
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
        case AdminActionTypes.CHANGE_LANGUAGE:
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

        case AdminActionTypes.CLEAR_ERROR:
            return {
                ...state,
                error: '',
            };

        case AdminActionTypes.UPDATE_DATA_ERROR:
            return {
                ...state,
                error: action.error.detail
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



        case AdminActionTypes.LOAD_IMPORT_JOB_DETAILS_DONE:
            return {
                ...state,
                import: {
                    ...state.import,
                    importJobDetails: action.importJobDetails
                },
            };

        case AdminActionTypes.LOAD_IMPORT_JOB_DETAILS_ERROR:
            return {
                ...state,
                import: {
                    ...state.import,
                    importJobDetails: undefined,
                },
            };

        case AdminActionTypes.LOAD_IPS_DONE:
            return {
                ...state,
                import: {
                    ...state.import,
                    importedPollingStations: action.payload,
                },
            };
        case AdminActionTypes.LOAD_IPS_ERROR:
            return {
                ...state,
                import: {
                    ...state.import,
                    importedPollingStations: undefined,
                },
            };

        case AdminActionTypes.RESET_FILTER:
            return {
                ...state,
                import: {
                    ...state.import,
                    importedPollingStationsFilter: {...DEFAULT_IPS_FILTER}
                }
            };

        case AdminActionTypes.UPDATE_FILTER:

            return {
                ...state,
                import: {
                    ...state.import,
                    importedPollingStationsFilter: action.payload
                }
            };

        case AdminActionTypes.UPDATE_PAGINATION:
            return {
                ...state,
                import: {
                    ...state.import,
                    importedPollingStationsPagination: action.payload
                }
            };
        default:
            return state;
    }
}
