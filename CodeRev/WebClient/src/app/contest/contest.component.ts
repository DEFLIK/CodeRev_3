import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { EditorMode } from '../code-editor/models/editorMode'; 
import { SavingService } from '../code-editor/services/saving-service/saving.service';
import { InterviewSolutionReview } from '../review/models/interviewSolutionReview';
import { ReviewService } from '../review/services/review.service';
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
    public isShowingEnd: boolean = false;
    public isShowingGrade: boolean = false;
    public get solutionInfo(): InterviewSolutionInfo | undefined {
        return this._contest.currentInterview;
    }
    public get isReview(): boolean {
        return this.editorMode === EditorMode.review;
    }

    public isStarted = false;
    public isDraftOpen = false;
    public isInfoHidden = false;
    public editType = EditorMode;
    public grade: number = 3;
    public comment: string = '';
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
        return this._contest.isSolutionComplete;
    }
    public get isAllTaskCompleted(): boolean {
        if (!this.taskList || this.taskList.tasks.length === 0) {
            return false;
        }

        return this.taskList.tasks.every(task => task.isDone);
    }
    constructor(
        private _contest: ContestService,
        private _saving: SavingService,
        private _review: ReviewService
    ) { }
    public ngAfterViewInit(): void {
        if (this.review) {
            this.taskList.loadInterviewTasks(this.review.interviewSolutionId, this.startTaskId);
            this.grade = this.review.averageGrade;
            this.comment = this.review.reviewerComment;
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

    public clickDraft(): void {
        this.isDraftOpen = !this.isDraftOpen;
    }

    public hideInfo(): void {
        this.isInfoHidden = !this.isInfoHidden;
    }

    public endSolution(): void {
        this.isShowingEnd = false;
        this._contest.endSolution()
            .subscribe(resp => {
                if (resp.ok && this._contest.currentTask) {
                    this._contest.selectTask(this._contest.currentTask);
                }
            });
    }

    public showEnd(show: boolean): void {
        this.isShowingEnd = show;
    }

    public showGrade(show: boolean): void {
        this.isShowingGrade = show;
    }

    public saveGrade(): void {
        this._review.setInterviewComment(this.review?.interviewSolutionId ?? 'govno', this.comment).subscribe();
        this._review.setInterviewGrade(this.review?.interviewSolutionId ?? 'govno', this.grade).subscribe();
    }
}
