import { Pipe, PipeTransform } from '@angular/core';
import { CandidateCardInfo } from '../models/candidateCardInfo';
import { CandidateFitlerCriteria } from '../models/candidateFilterCriteria';
import { CandidateStateKeys, CandidateState } from '../models/candidateState';
import { CandidateVacancy, CandidateVacancyKeys } from '../models/candidateVacancy';
import { MeetInfo } from '../models/meetInfo';

@Pipe({
    name: 'meetsFilter'
})
export class MeetsFilterPipe implements PipeTransform {

    public transform(value: MeetInfo[], serachCriteria: string): MeetInfo[] {
        return value
            .filter((meet: MeetInfo) => 
                (meet.firstName.toLowerCase().includes(serachCriteria.toLowerCase()) 
                    || meet.surname.toLowerCase().includes(serachCriteria.toLowerCase()) 
                    || meet.vacancy.toLowerCase().includes(serachCriteria.toLowerCase()))
                    || meet.programmingLanguage.toLocaleLowerCase().includes(serachCriteria.toLocaleLowerCase()));
        // .sort((card, next) => (card.startTimeMs - next.startTimeMs) * (filterCriteria.descending ? 1 : -1));
        // добавить сортировку по времени встречи
    }

}
