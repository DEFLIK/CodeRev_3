import { Pipe, PipeTransform } from '@angular/core';
import { TaskInfo } from '../models/taskInfo';
import { InterviewInfo } from '../models/interviewInfo';

@Pipe({
    name: 'interviewFilter'
})
export class InterviewFilterPipe implements PipeTransform {

    public transform(value: InterviewInfo[], searchCriteria: string): InterviewInfo[] {
        return value
            .filter((int: InterviewInfo) => 
                int.interviewText.toLowerCase().includes(searchCriteria.toLowerCase()) ||
                int.vacancy.toLowerCase().includes(searchCriteria.toLowerCase()) ||
                int.interviewText.toLowerCase().includes(searchCriteria.toLowerCase()));
    }

}
