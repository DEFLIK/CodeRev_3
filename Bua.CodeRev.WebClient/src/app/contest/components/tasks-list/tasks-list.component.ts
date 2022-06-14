import { Component, Input, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { TaskSolutionInfo } from '../../models/taskSolutionInfo';
import { ContestService } from '../../services/contest-service/contest.service';

@Component({
    selector: 'app-tasks-list',
    templateUrl: './tasks-list.component.html',
    styleUrls: ['./tasks-list.component.less']
})
export class TasksListComponent {
    public tasks: TaskSolutionInfo[] = [];
    public currentTask?: TaskSolutionInfo;
    constructor(private _contest: ContestService) { }

    public change(task: TaskSolutionInfo): void {
        this.currentTask = task;
        this._contest.selectTask(task);
    }

    public loadInterviewTasks(slnId: string, startTaskId: string = ''): void {
        this._contest
            .getTasksInfo(slnId)
            .subscribe({
                next: (resp) => {
                    if (resp.ok && resp.body) {
                        this.tasks = [];
                        for (const task of resp.body) {
                            this.tasks.push(new TaskSolutionInfo(task));
                        }         
                    }

                    if (this.tasks.length > 0) {
                        setTimeout(() => { 
                            const startTask = this.tasks.find(task => task.id === startTaskId) ?? this.tasks[0];
                            this._contest.selectTask(startTask);
                            this.currentTask = startTask;
                        });
                    }
                }
            });
    }

}
