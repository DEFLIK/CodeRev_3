import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import {interval, Observable, Subject, Subscription, takeUntil} from 'rxjs';
import { EntryPoint } from 'src/app/code-editor/models/entryPoint';
// import { CodeStorageService } from 'src/app/code-editor/services/storage-service/code-storage.service';
import { CompileService } from 'src/app/code-editor/services/compile-service/compile-service.service';
import { RecordService } from 'src/app/code-editor/services/record-service/record.service';
import { RecordInfo } from '../../models/codeRecord';
import { EditorMode } from '../../models/editorMode';
import { PlayerService } from '../../services/player-service/player.service';
import { PatchedTimelineComponent } from '../patched-timeline/patched-timeline.component';
import { SavingService } from '../../services/saving-service/saving.service';
import { TaskSolutionInfo } from 'src/app/contest/models/taskSolutionInfo';
import { ReviewService } from 'src/app/review/services/review.service';
import { SaveChunk } from '../../models/saveChunk';
import { ContestService } from 'src/app/contest/services/contest.service';

@Component({
    selector: 'app-controls',
    templateUrl: './controls.component.html',
    styleUrls: ['./controls.component.less'],
    // changeDetection: ChangeDetectionStrategy.OnPush
})
export class ControlsComponent implements OnInit, OnDestroy {
    // @ViewChild('timeline')
    // public timeline?: ElementRef;
    // @ViewChild('nxgtimeline')
    // public timelineComp!: NgxVideoTimelineComponent;
    @ViewChild('timeline')
    public patchedTimeline!: PatchedTimelineComponent;
    @Input()
    public editorMode!: EditorMode;
    @Input()
    public taskSelected$!: Observable<TaskSolutionInfo>;
    // public startTime: number = new Date().getTime();
    public get isPlaying(): boolean {
        return this._player.isPlaying;
    }
    public readOnly = false;
    // public get width(): number {
    //     console.log(this.timeline?.nativeElement.offsetWidth);

    //     return this.timeline?.nativeElement.offsetWidth ?? 0;
    // }
    private _bindedEditor?: CodemirrorComponent;
    _currentTask?: TaskSolutionInfo;
    private _unsubscriber = new Subject<void>();
    private _timeLineUpdateSubscription?: Subscription;

    constructor(
        private _compiler: CompileService,
        private _record: RecordService,
        private _saving: SavingService,
        private _player: PlayerService,
        private _contest: ContestService,
        private _review: ReviewService
    ) {
    }

    public ngOnDestroy(): void {
        this._unsubscriber.next();
    }
    public ngOnInit(): void {
        // Перетащить в место получше
        this.taskSelected$
            .pipe(takeUntil(this._unsubscriber))
            .subscribe(nextTask => this.changeTask(nextTask));
    }

    public bindToEditor(editor: CodemirrorComponent): void {
        this._bindedEditor = editor;

        if (this.editorMode === EditorMode.write) {
            this._record.bindEditor(editor);
            this._record.enablePageChangesCheck();
        }

        if (this.editorMode === EditorMode.review) {
            this._player.bindEditor(editor);
            this.patchedTimeline.buildComponent();
        }
    }

    public run(): void {
        if (!this._currentTask || this._currentTask.runAttemptsLeft === 0) {
            return;
        }

        this._compiler
            .execute(
                this._bindedEditor?.codeMirror?.getValue() ?? '',
                new EntryPoint(
                    'CodeRev',
                    'Program',
                    'Main'));

        this._contest
            .reduceTaskSolutionAttempt(this._currentTask.id)
            .subscribe({
                next: (resp) => {
                    if (resp.ok && resp.body) {
                        this._currentTask!.runAttemptsLeft = resp.body; //todo не обновляет динамически, надо починить
                    }
                }
            });
    }

    public startRecord(): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this._record.startRecord(this._currentTask.id);
    }

    public stopRecord(): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this._record.stopRecord(this._currentTask.id);
    }

    public save(continueRecord: boolean): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this.stopRecord();

        this._saving.saveNext(
            this._currentTask.id,
            this._bindedEditor.codeMirror?.getValue() ?? '',
            this._record.getTaskRecord(this._currentTask.id));

        if (continueRecord) {
            this.startRecord();
        }
    }

    public playSavedRecord(): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this._player.play();
    }

    public pauseSavedRecord(): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this._player.pause();
    }

    public seek(time: number): void {
        this._player.seek(time);
    }

    public clear(): void {
        this._player.clear();
    }

    private updateTimeLine(task: TaskSolutionInfo): void {
        const saves = this._saving.getTaskSaves(task.id);
        this._timeLineUpdateSubscription?.unsubscribe();

        if (saves.length !== 0) {
            this._player.selectSavesRecords(saves);
            this.patchedTimeline.setProperties(
                saves[0].recordInfo.recordStartTime,
                this._player.getSaveDuration(),
                this._player.getSaveRecords());

            this._timeLineUpdateSubscription = interval(100)
                .pipe(takeUntil(this._unsubscriber))
                .subscribe(() => {
                    if (this._player.currentRecord) {
                        this.patchedTimeline.updateOperationLabel(this._player.currentRecord);
                    }

                    if (this._player.isPlaying) {
                        // this.patchedTimeline.timeLineComp!.isPlayClick = true;
                        this.patchedTimeline.setCurrentTime(this._player.getCurrentTime());
                    } else {
                        // this.patchedTimeline.timeLineComp!.isPlayClick = false;
                    }
                });
        } else {
            this._player.clear();
            this.patchedTimeline.setProperties(
                Date.now(),
                1000000,
                []
            );
            this._player.selectSavesRecords([
                new SaveChunk(
                    task.id,
                    Date.now(),
                    '',
                    new RecordInfo([], Date.now()))]);
            this._bindedEditor?.codeMirror?.setValue('');
        }
    }

    private changeTask(nextTask: TaskSolutionInfo): void {
        switch (this.editorMode) {
            case (EditorMode.write):
                const isEditable = !nextTask.isDone && !this._contest.isSolutionExpired && !this._contest.isSolutionComplete;

                if (!this._bindedEditor) {
                    console.log('Cant resolve binded editor');
                    break;
                }

                if (isEditable) {
                    this.save(false);
                }

                if (!isEditable) {
                    this._bindedEditor.setDisabledState(true);
                    this.readOnly = true;
                } else {
                    this.readOnly = false;
                    this._bindedEditor.setDisabledState(false);
                }

                this._bindedEditor.codeMirror?.setValue(this._saving.getLastSavedCode(nextTask.id) ?? nextTask.startCode);

                if (isEditable) {
                    this._record.startRecordingTask(nextTask.id);
                }
                break;
            case (EditorMode.review):
                this.updateTimeLine(nextTask);
                break;
        }

        this._currentTask = nextTask;
    }
}
