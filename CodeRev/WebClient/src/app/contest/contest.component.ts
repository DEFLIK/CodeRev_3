import { AfterContentInit, AfterViewInit, Component, ComponentFactoryResolver, Input, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { flatMap, forkJoin, Observable, Subject, zip } from 'rxjs';
import { EditorMode } from '../code-editor/models/editorMode'; 
import { SavingService } from '../code-editor/services/saving-service/saving.service';
import { InterviewSolutionReview } from '../review/models/interviewSolutionReview';
import { TasksListComponent } from './components/tasks-list/tasks-list.component';
import { InterviewSolutionInfo } from './models/interviewSolutionInfo';
import { TaskSolutionInfo } from './models/taskSolutionInfo';
import { ContestService } from './services/contest-service/contest.service';

@Component({
    selector: 'app-contest',
    templateUrl: './contest.component.html',
    styleUrls: ['./contest.component.less']
})
export class ContestComponent implements AfterViewInit {
    @Input()
    public editorMode = EditorMode.write;
    @Input()
    public review?: InterviewSolutionReview;
    @Input()
    public startTaskId?: string;
    public isStarted = false;
    public isDraftOpen = false;
    public editType = EditorMode;
    public get taskSelected$(): Observable<TaskSolutionInfo> {
        return this._contest.taskSelected$;
    }
    // @ViewChild('container', { read: ViewContainerRef })
    // public container!: ViewContainerRef;
    @ViewChild('list')
    public taskList!: TasksListComponent;
    public get isSolutionExpired(): boolean {
        return this._contest.isSolutionExpired;
    }
    public get isSolutionComplete(): boolean {
        if (!this.taskList || this.taskList.tasks.length === 0) {
            return false;
        }

        return this.taskList.tasks.every(task => task.isDone);
    }
    constructor(
        private _contest: ContestService,
        private _saving: SavingService
    ) { }
    public ngAfterViewInit(): void {
        if (this.review) {
            this.taskList.loadInterviewTasks(this.review.interviewSolutionId, this.startTaskId);
        }
    }

    public start(sln: InterviewSolutionInfo): void {
        // this.isStarted = true;
        this._contest
            .startInterview(sln.id)
            .subscribe({
                next: (resp) => {
                    if (resp.ok) {
                        // this.createList(sln.id);
                        this.continue(sln);
                    }
                }
            });
    }

    public continue(sln: InterviewSolutionInfo): void {
        this.isStarted = true;
        this._contest.continueInterview(sln);
        this.taskList.loadInterviewTasks(sln.id);
    }

    public applyTaskError(task: TaskSolutionInfo): void {
        
    }

    public swapDraft(): void {
        this.isDraftOpen = !this.isDraftOpen;
    }

    // private createList(interviewSolutionId: string): void {
    //     this.isStarted = true;
    //     this.interviewSolutionId = interviewSolutionId;
    //     this.container.clear();
    //     const comp = this._componentFactory
    //         .resolveComponentFactory(
    //             TasksListComponent
    //         );
    //     const compRef = this.container.createComponent(comp);
    //     compRef.instance.interviewSolutionId = interviewSolutionId;
    // }
}
