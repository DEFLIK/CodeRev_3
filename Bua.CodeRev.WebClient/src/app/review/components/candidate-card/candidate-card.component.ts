import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { CandidateCardInfo } from '../../models/candidateCardInfo';

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
    constructor() { }

    @HostListener('click')
    public onClick(): void {
        this.clickEvent.emit(this.candidate);
    }


}
