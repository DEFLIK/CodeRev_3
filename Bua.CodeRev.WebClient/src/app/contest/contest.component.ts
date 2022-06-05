import { Component, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { EditorMode } from '../code-editor/models/editorMode'; 
import { ContestService } from './services/contest-service/contest.service';

@Component({
    selector: 'app-contest',
    templateUrl: './contest.component.html',
    styleUrls: ['./contest.component.less']
})
export class ContestComponent {
    public types = EditorMode;
    public get taskSelected$(): Observable<string> {
        return this._contest.taskSelected$;
    }
    constructor(private _contest: ContestService) { }
}
