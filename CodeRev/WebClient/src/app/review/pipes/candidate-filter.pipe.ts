import { Pipe, PipeTransform } from '@angular/core';
import { CandidateCardInfo } from '../models/candidateCardInfo';
import { CandidateFilterCriteria } from '../models/candidateFilterCriteria';
import { CandidateStateKeys, CandidateState } from '../models/candidateState';
import { convertProgrammingLanguageToString } from "../models/programmingLanguage";

@Pipe({
    name: 'candidateFilter'
})
export class CandidateFilterPipe implements PipeTransform {

    public transform(value: CandidateCardInfo[], filterCriteria: CandidateFilterCriteria, searchCriteria: string): CandidateCardInfo[] {
        return value
            .filter((card: CandidateCardInfo) =>
                (card.firstName.toLowerCase().includes(searchCriteria.toLowerCase())
                    || card.surname.toLowerCase().includes(searchCriteria.toLowerCase())
                    || card.vacancy.toLowerCase().includes(searchCriteria.toLowerCase())
                    || card.programmingLanguages.map(language => convertProgrammingLanguageToString(language))
                        .includes(searchCriteria.toLocaleLowerCase()))
                && (filterCriteria.programmingLanguages.every(lang =>
                        card.programmingLanguages.map(lang => convertProgrammingLanguageToString(lang)).includes(lang)))
                && (filterCriteria.vacancies.some(vacan => vacan === card.vacancy)
                    || filterCriteria.vacancies.length === 0)
                && (filterCriteria.states.some(state => CandidateState[state as CandidateStateKeys] === card.getState())
                    || filterCriteria.states.length === 0))
            .sort((card, next) => (card.startTimeMs - next.startTimeMs) * (filterCriteria.descending ? 1 : -1));
    }

}
