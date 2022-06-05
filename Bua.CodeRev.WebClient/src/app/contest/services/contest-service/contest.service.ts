import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { TaskInfoResponse } from 'src/app/review/models/response/taskInfo-response';
import { TaskInfo } from 'src/app/review/models/taskInfo';

@Injectable({
    providedIn: 'root'
})
export class ContestService {
    public get taskSelected$(): Observable<string> {
        return this._taskSelected$.asObservable();
    }
    public get isTaskSelected(): boolean {
        return !!this._currentTask;
    }
    private _tasks = ['task1', 'task2'];
    private _currentTask?: string;
    private _taskSelected$ = new Subject<string>();
    
    constructor() { }

    public getTasks(): string[] {
        return this._tasks;
    }

    public selectTask(task: string): void {
        this._taskSelected$.next(task);
    }
}
