import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MeetInfo } from '../../models/meetInfo';
import { convertProgrammingLanguageToString, ProgrammingLanguage } from '../../models/programmingLanguage';

@Component({
    selector: 'app-meet-card',
    templateUrl: './meet-card.component.html',
    styleUrls: ['./meet-card.component.less']
})
export class MeetCardComponent {

    @Input()
    public meetInfo!: MeetInfo;
    @Output()
    public selectMeetEvent: EventEmitter<MeetInfo> = new EventEmitter();

    constructor() { }

    public convertProgrammingLanguageToString(language: ProgrammingLanguage): string {
        return convertProgrammingLanguageToString(language);
    }
}
