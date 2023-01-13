import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import * as CodeMirror from 'codemirror';
import { interval, Observable } from 'rxjs';
import { CodeEditorComponent } from '../code-editor/code-editor.component';
import { EditorMode } from '../code-editor/models/editorMode'; 
import { SavingService } from '../code-editor/services/saving-service/saving.service';
import { InterviewSolutionReview } from '../review/models/interviewSolutionReview';
import { ReviewService } from '../review/services/review.service';
import { SignalrComponent } from '../webcam/components/signalr.component';
import { RtcService } from '../webcam/services/rtc.service';
import { SignalrService } from '../webcam/services/signalr.service';
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
    @ViewChild('editor')
    public codeEditor?: CodeEditorComponent;
    @ViewChild('signalR')
    public signalR?: SignalrComponent;
    public isShowingEnd: boolean = false;
    public isShowingGrade: boolean = false;
    public get solutionInfo(): InterviewSolutionInfo | undefined {
        return this._contest.currentInterview;
    }
    public get isReview(): boolean {
        return this.editorMode === EditorMode.review;
    }
    public get solutionId(): string | undefined {
        if (this.isReview) {
            return this.review?.interviewSolutionId;
        }

        return this.solutionInfo?.id;
    }
    public isStarted = false;
    public isDraftOpen = false;
    public isInfoHidden = false;
    public editType = EditorMode;
    public grade: number = 3;
    public comment: string = '';
    public isSync: boolean;
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
        private _review: ReviewService,
        private _route: ActivatedRoute

    ) { 
        this.isSync = 
            this._route.snapshot.paramMap.get('state') === 'sync' ||
            (this.editorMode === EditorMode.write && !this.isSolutionComplete && !this.isSolutionExpired);
    }
    public ngAfterViewInit(): void {
        if (this.review) {
            this.taskList.loadInterviewTasks(this.review.interviewSolutionId, this.startTaskId);
            this.grade = this.review.averageGrade;
            this.comment = this.review.reviewerComment;
        }

        if (this.editorMode !== EditorMode.review) {
            interval(100).subscribe(t => {
                var code = this.codeEditor?.codeMirrorCmpt.codeMirror?.getValue() ?? 'EMPTY DATA SENT';

                this.signalR?.sendData(code);
            });
        }

        this.signalR?.signalR.data$.subscribe(data => { //todo unsub
            this.codeEditor?.codeMirrorCmpt.codeMirror?.setValue(data ?? 'CANT READ DATA');
        });
    }

    public updateCode(code: string): void {
        this.codeEditor?.codeMirrorCmpt.codeMirror?.setValue(code);
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
