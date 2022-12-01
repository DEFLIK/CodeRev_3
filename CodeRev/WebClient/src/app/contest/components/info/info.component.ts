import { Component, Input, OnInit } from '@angular/core';
import { CandidateCardInfo } from 'src/app/review/models/candidateCardInfo';
import { InterviewSolutionReview } from 'src/app/review/models/interviewSolutionReview';

@Component({
    selector: 'app-info',
    templateUrl: './info.component.html',
    styleUrls: ['./info.component.less']
})
export class InfoComponent {

    @Input()
    public review!: InterviewSolutionReview;
    @Input()
    public cardInfo!: CandidateCardInfo;
    constructor() { }

}
