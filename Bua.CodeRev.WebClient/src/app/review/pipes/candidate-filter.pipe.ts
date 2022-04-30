import { Pipe, PipeTransform } from '@angular/core';
import { CandidateCardInfo } from '../models/candidateCardInfo';

@Pipe({
    name: 'candidateFilter'
})
export class CandidateFilterPipe implements PipeTransform {

    public transform(value: CandidateCardInfo[], ...args: unknown[]): CandidateCardInfo[] {
        return value;
    }

}
