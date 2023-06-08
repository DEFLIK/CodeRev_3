import { Pipe, PipeTransform } from '@angular/core';
import { MeetInfo } from '../models/meetInfo';
import {MeetFilterCriteria} from "../models/meetFilterCriteria";
import { convertProgrammingLanguageToString } from "../models/programmingLanguage";

@Pipe({
    name: 'meetsFilter'
})
export class MeetsFilterPipe implements PipeTransform {

    public transform(value: MeetInfo[], filterCriteria: MeetFilterCriteria, searchCriteria: string): MeetInfo[] {
        const filteredMeets = value
            .filter((meet: MeetInfo) =>
                (meet.firstName.toLowerCase().includes(searchCriteria.toLowerCase())
                    || meet.surname.toLowerCase().includes(searchCriteria.toLowerCase())
                    || meet.vacancy.toLowerCase().includes(searchCriteria.toLowerCase())
                    || meet.programmingLanguages.map(language => convertProgrammingLanguageToString(language))
                        .includes(searchCriteria.toLocaleLowerCase())) // todo не конвертить каждый раз в строку, то же в candidate-filter.pipe.ts
                && (filterCriteria.programmingLanguages.every(lang =>
                        meet.programmingLanguages.map(lang => convertProgrammingLanguageToString(lang)).includes(lang)))
                && (filterCriteria.vacancies.some(vacancy => vacancy === meet.vacancy)
                    || filterCriteria.vacancies.length === 0));
        return filterCriteria.myMeetsFirst
            ? filteredMeets.sort((m1, m2) => Number(m2.isOwnerMeet) - Number(m1.isOwnerMeet))
            : filteredMeets;
        // .sort((card, next) => (card.startTimeMs - next.startTimeMs) * (filterCriteria.descending ? 1 : -1));
        // добавить сортировку по времени встречи
    }

}
