import { Component, Input, OnInit } from '@angular/core';
import { MeetInfo } from '../../models/meetInfo';

@Component({
    selector: 'app-meet-card',
    templateUrl: './meet-card.component.html',
    styleUrls: ['./meet-card.component.less']
})
export class MeetCardComponent {

    @Input()
    public meetInfo!: MeetInfo;

    constructor() { }

}
