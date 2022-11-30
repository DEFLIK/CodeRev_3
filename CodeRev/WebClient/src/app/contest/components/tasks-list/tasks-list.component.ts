import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { catchError, first, forkJoin, Observable, of, Subject, Subscription, take, zip } from 'rxjs';
import { EditorMode } from 'src/app/code-editor/models/editorMode';
import { SavingService } from 'src/app/code-editor/services/saving-service/saving.service';
import { TaskSolutionInfo } from '../../models/taskSolutionInfo';
import { ContestService } from '../../services/contest-service/contest.service';

@Component({
    selector: 'app-tasks-list',
    templateUrl: './tasks-list.component.html',
    styleUrls: ['./tasks-list.component.less']
})
export class TasksListComponent {
    @Input()
    public editorMode: EditorMode = EditorMode.write;
    public tasks: TaskSolutionInfo[] = [];
    public currentTask?: TaskSolutionInfo;
    public tasksStates = new Map<string, boolean>();
    @Output()
    public taskLoadingError = new EventEmitter<TaskSolutionInfo>();
    @Output()
    public draftButtonClick = new EventEmitter<boolean>();
    constructor(
        private _contest: ContestService,
        private _saving: SavingService
    ) { }

    public openTask(task: TaskSolutionInfo): void {
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
                        const obs = [];

                        for (const task of resp.body) {
                            const taskModel = new TaskSolutionInfo(task);
                            this.tasks.push(taskModel);
                            const stream = this._contest
                                .getLastSavedCode(taskModel.id)
                                .pipe(catchError(() => of(undefined)));
                            obs.push(stream);
                            stream
                                .subscribe({
                                    next: save => {
                                        if (save?.ok && save.body) {
                                            if (save.body.code) {
                                                this._saving.setSavedCode(taskModel.id, save.body.code);
                                                this.tasksStates.set(taskModel.id, true);
                                            }
                                        } else {
                                            this.taskLoadingError.emit(taskModel);
                                        }
                                        this.tasksStates.set(taskModel.id, true);
                                    }
                                });
                        }
                        
                        forkJoin(obs)
                            .subscribe({
                                next: () => {
                                    const startTask = this.tasks.find(task => task.id === startTaskId && this.tasksStates.get(task.id));
                                    const anyLoaded = this.tasks.find(task => this.tasksStates.get(task.id));

                                    setTimeout(() => { // Необходимо закинуть в конец очереди для пропуска загрузки редактора кода
                                        if (!startTask && anyLoaded) {
                                            this.selectStartTask(anyLoaded);
                                        } else if (startTask) {
                                            this.selectStartTask(startTask);
                                        }
                                    });
                                }
                            });
                    }
                }
            });
    }

    private selectStartTask(startTask: TaskSolutionInfo): void {
        this._contest.selectTask(startTask);
        this.currentTask = startTask;
    }
}
