import { AfterViewInit, Component, Input, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import * as CodeMirror from 'codemirror';
import { interval, Observable, Subject, takeUntil } from 'rxjs';
import { CodeEditorComponent } from '../code-editor/code-editor.component';
import { EditorMode } from '../code-editor/models/editorMode'; 
import { CompileService } from '../code-editor/services/compile-service/compile-service.service';
import { SavingService } from '../code-editor/services/saving-service/saving.service';
import { MeetPeerData } from '../global-services/request/models/meet-peer-data';
import { InterviewSolutionReview } from '../review/models/interviewSolutionReview';
import { ReviewService } from '../review/services/review.service';
import { SignalrComponent } from '../webcam/components/signalr.component';
import { RtcService } from '../webcam/services/rtc.service';
import { SignalrService } from '../webcam/services/signalr.service';
import { TasksListComponent } from './components/tasks-list/tasks-list.component';
import { InterviewSolutionInfo } from './models/interviewSolutionInfo';
import { TaskSolutionInfo } from './models/taskSolutionInfo';
import { ContestService } from './services/contest.service';
import { TestsRunnerService } from '../code-editor/services/tests-runner-service/tests-runner.service';

@Component({
    selector: 'app-contest',
    templateUrl: './contest.component.html',
    styleUrls: ['./contest.component.less']
})
export class ContestComponent implements AfterViewInit, OnDestroy {
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
    public get isSyncReview(): boolean {
        return (this.editorMode === EditorMode.review && this._route.snapshot.paramMap.get('state') === 'sync');
    };
    public isSyncWrite = false;
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
    private _unsubscriber = new Subject<void>();


    constructor(
        private _contest: ContestService,
        private _saving: SavingService,
        private _review: ReviewService,
        private _route: ActivatedRoute,
        private _compiler: CompileService,
        private _testsRunner: TestsRunnerService

    ) { 
    }
    public ngAfterViewInit(): void {
        if (this.review) {
            this.taskList.loadInterviewTasks(this.review.interviewSolutionId, this.startTaskId);
            this.grade = this.review.averageGrade;
            this.comment = this.review.reviewerComment;

            if (this.editorMode === EditorMode.review && this.isSyncReview) {
                this.signalR?.signalR.data$
                    .pipe(takeUntil(this._unsubscriber))
                    .subscribe(data => { // todo unsub
                        if (data.codeUpdate !== undefined) {
                            this.codeEditor!.codeMirrorCmpt.codeMirror!.setValue(data.codeUpdate);
                        }
                        if (data.taskIdUpdate) {
                            this.taskList.openTask(this.taskList.tasks.filter(task => task.id === data.taskIdUpdate)[0], true);
                        }
                        if (data.consoleUpdate) {
                            this._compiler.emitOutput(data.consoleUpdate);
                        }
                        if (data.testsUpdate) {
                            this._testsRunner.emitOutput(data.testsUpdate);
                        }
                    });
            }
        }

        console.log(this);
        
    }

    public ngOnDestroy(): void {
        this._unsubscriber.next();
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
        this.isSyncWrite = sln.isSynchronous && !this._contest.isSolutionComplete && !this._contest.isSolutionExpired;

        this._contest.taskSelected$
            .pipe(takeUntil(this._unsubscriber))
            .subscribe(task => {
                var meetData = new MeetPeerData();
                meetData.taskIdUpdate = task.id;

                this.signalR?.sendData(JSON.stringify(meetData));
            });

        this._compiler.onOutputRefresh$
            .pipe(takeUntil(this._unsubscriber))
            .subscribe(output => {
                var meetData = new MeetPeerData();
                meetData.consoleUpdate = output;

                this.signalR?.sendData(JSON.stringify(meetData));
            });
            
        this._testsRunner.onOutputRefresh$
            .pipe(takeUntil(this._unsubscriber))
            .subscribe(output => {
                var meetData = new MeetPeerData();
                meetData.testsUpdate = output;

                this.signalR?.sendData(JSON.stringify(meetData));
            });
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
        this.isShowingGrade = false;
    }

    public onCodeChanged(code: string): void {
        // if (this.editorMode === EditorMode.write && sln.isSynchronous) {
        var meetData = new MeetPeerData();
        meetData.codeUpdate = code;

        this.signalR?.sendData(JSON.stringify(meetData));
        // }
    }
}
