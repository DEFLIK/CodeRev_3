import { Pipe, PipeTransform } from '@angular/core';
import { CandidateCardInfo } from '../models/candidateCardInfo';
import { CandidateFitlerCriteria } from '../models/candidateFilterCriteria';
import { CandidateStateKeys, CandidateState } from '../models/candidateState';
import { CandidateVacancy, CandidateVacancyKeys } from '../models/candidateVacancy';

@Pipe({
    name: 'candidateFilter'
})
export class CandidateFilterPipe implements PipeTransform {

    public transform(value: CandidateCardInfo[], filterCriteria: CandidateFitlerCriteria, serachCriteria: string): CandidateCardInfo[] {
        return value
            .filter((card: CandidateCardInfo) => 
                (card.fullName.toLowerCase().includes(serachCriteria.toLowerCase()) 
                    || card.vacancy.toLowerCase().includes(serachCriteria.toLowerCase()))
                && (filterCriteria.vacancies.some(vacan => vacan === card.vacancy) 
                    || filterCriteria.vacancies.length === 0)
                && (filterCriteria.states.some(state => CandidateState[state as CandidateStateKeys] === card.getState())
                    || filterCriteria.states.length === 0))
            .sort((card, next) => (card.startTimeMs - next.startTimeMs) * (filterCriteria.descending ? 1 : -1));
    }

}
