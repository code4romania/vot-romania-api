import { AppActions, ActionTypes } from "./actions";
import { VotingGuide, AppData } from "../services/data.service";

export interface ApplicationState {
    languages: string[];
    generalInfo: string;
    selectedLanguage: string;
    votingGuide: VotingGuide;
    data: AppData[];
    error: string;
}

const initialState: ApplicationState = {
    languages: [],
    votingGuide: undefined,
    data: undefined,
    error: '',
    generalInfo: '',
    selectedLanguage: 'Ro' // change to enum
};
export function appStateReducer(state: ApplicationState = initialState, action: AppActions): ApplicationState {
    switch (action.type) {
        case ActionTypes.LOAD_DATA_DONE:
            const languageData = action.payload.data.find(x => x.language === state.selectedLanguage);
            if (languageData === undefined) {
                return state;
            }

            return {
                ...state,
                languages: action.payload.data.map(x => x.language),
                data: action.payload.data,
                generalInfo: languageData.generalInfo,
                votingGuide: languageData.votersGuide
            };
        case ActionTypes.CHANGE_LANGUAGE:
            const changedLanguageData = state.data.find(x => x.language === action.payload);

            if (changedLanguageData === undefined) {
                return state;
            }

            return {
                ...state,
                generalInfo: changedLanguageData.generalInfo,
                votingGuide: changedLanguageData.votersGuide,
                selectedLanguage: action.payload
            };
          

        default:
            return state;
    }
}
