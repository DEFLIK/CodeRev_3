import { Component, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { EditorMode } from '../code-editor/models/editorMode'; 

@Component({
    selector: 'app-contest',
    templateUrl: './contest.component.html',
    styleUrls: ['./contest.component.less']
})
export class ContestComponent {
    public types = EditorMode;
    public get taskSelected$(): Observable<string> {
        return this._taskSelected$.asObservable();
    }
    public get isTaskSelected(): boolean {
        return !!this._currentTask;
    }
    public tasks = ['task1', 'task2'];
    private _currentTask?: string;
    private _taskSelected$ = new Subject<string>();
    constructor() { }

    public change(task: string): void {
        this._taskSelected$.next(task);
    }

}
