import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { CandidateCardInfo } from '../../models/candidateCardInfo';
import { CandidateState, CandidateStateKeys } from '../../models/candidateState';

@Component({
    selector: 'app-candidate-card',
    templateUrl: './candidate-card.component.html',
    styleUrls: ['./candidate-card.component.less']
})
export class CandidateCardComponent {
    @Input()
    public candidate!: CandidateCardInfo;
    @Output()
    public clickEvent: EventEmitter<CandidateCardInfo> = new EventEmitter<CandidateCardInfo>();
    public state = CandidateState;
    public isShowingComment = false;
    constructor() { }

    public select(): void {
        this.clickEvent.emit(this.candidate);
    }

    public showComment(): void {
        this.isShowingComment = true;
    }

    public closeComment(): void {
        this.isShowingComment = false;
    }


}
