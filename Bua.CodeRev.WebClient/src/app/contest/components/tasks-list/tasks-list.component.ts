import { Component, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { TaskInfo } from 'src/app/review/models/taskInfo';
import { ContestService } from '../../services/contest-service/contest.service';

@Component({
    selector: 'app-tasks-list',
    templateUrl: './tasks-list.component.html',
    styleUrls: ['./tasks-list.component.less']
})
export class TasksListComponent {
    public get tasks(): string[] {
        return this._contest.getTasks();
    }
    
    constructor(private _contest: ContestService) { }

    public change(task: string): void {
        this._contest.selectTask(task);
    }

}
