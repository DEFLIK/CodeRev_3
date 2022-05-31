import { AfterViewChecked, AfterViewInit, ChangeDetectionStrategy, Component, ComponentFactoryResolver, ElementRef, HostListener, Input, OnDestroy, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { CanvasPos, NgxVideoTimelineComponent, VideoCellType } from 'ngx-video-timeline';
import { interval, Observable, Subscription } from 'rxjs';
import { EntryPoint } from 'src/app/code-editor/models/entryPoint';
import { CodeStorageService } from 'src/app/code-editor/services/storage-service/code-storage.service';
import { CompileService } from 'src/app/code-editor/services/compile-service/compile-service.service';
import { RecordService } from 'src/app/code-editor/services/record-service/record.service';
import { IOperationMark, RecordInfo } from '../../models/codeRecord';
import { EditorMode } from '../../models/editorMode';
import { ExecutionResult } from '../../models/executionResult';
import { ExecutionResultResponse } from '../../models/response/executionResult-response';
import { PlayerService } from '../../services/player-service/player.service';
import { PatchedTimelineComponent } from '../patched-timeline/patched-timeline.component';
import { SavingService } from '../../services/saving-service/saving.service';

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
    public taskSelected$!: Observable<string>;
    // public startTime: number = new Date().getTime();
    public get isPlaying(): boolean {
        return this._player.isPlaying;
    }
    // public get width(): number {
    //     console.log(this.timeline?.nativeElement.offsetWidth);
        
    //     return this.timeline?.nativeElement.offsetWidth ?? 0;
    // }
    private _bindedEditor?: CodemirrorComponent;
    private _rangeUpdater?: Subscription;
    private _currentTask?: string;
    private _taskUpdater?: Subscription;

    constructor(
        private _compiler: CompileService,
        private _record: RecordService,
        private _saving: SavingService,
        private _player: PlayerService,
        private _codeStorage: CodeStorageService,
    ) {
    }

    public ngOnDestroy(): void {
        this._rangeUpdater?.unsubscribe();
        this._taskUpdater?.unsubscribe();
    }
    public ngOnInit(): void {
        this._taskUpdater = this.taskSelected$
            .subscribe(taskId => {
                if (this.editorMode === EditorMode.write) {
                    this.save();
                    this._bindedEditor?.codeMirror?.setValue(this._saving.getLastSave(taskId)?.code ?? this._bindedEditor?.codeMirror?.getValue());
                    this._record.changeRecordingTask(taskId);
                }

                if (this.editorMode === EditorMode.review) {
                    const saves = this._saving.getTaskSaves(taskId);
                    // console.log(saves);
                    

                    this._player.selectSavesRecords(saves);
                    this.patchedTimeline.setProperties(
                        saves[0].record.recordStartTime, 
                        this._player.getSaveDuration(), 
                        this._player.getSaveRecords());

                    this._rangeUpdater = interval(100)
                        .subscribe(() => {
                            if (this._player.isPlaying) {
                                // this.patchedTimeline.timeLineComp!.isPlayClick = true;
                                this.patchedTimeline.setCurrentTime(this._player.getCurrentTime());
                            } else {
                                // this.patchedTimeline.timeLineComp!.isPlayClick = false;
                            }
                        });
                }

                this._currentTask = taskId;
            });
    }

    public bindToEditor(editor: CodemirrorComponent): void {
        this._bindedEditor = editor;

        if (this.editorMode === EditorMode.write) {
            this._record.bindEditor(editor);
        }

        if (this.editorMode === EditorMode.review) {
            this._player.bindEditor(editor);
            this.patchedTimeline.buildComponent();
        }
    }

    public run(): void {
        if (!this._currentTask) {
            return;
        }

        // this._codeStorage.saveCode(this._currentTask, this._bindedEditor?.codeMirror?.getValue() ?? '');

        this._compiler
            .execute(
                this._codeStorage.getSavedCode(this._currentTask),
                new EntryPoint(
                    this._codeStorage.entryNamespace, 
                    this._codeStorage.entryClass, 
                    this._codeStorage.entryMethod))
            .subscribe(resp => {
                if (resp.ok) {
                    this._codeStorage.storeOutput(resp.body ?? new ExecutionResult(new ExecutionResultResponse()));
                } else {
                    //todo
                }
            });
    }

    public startRecord(): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this._record.startRecord(this._currentTask);
    }

    public stopRecord(): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this._record.stopRecord(this._currentTask);
    }

    public save(): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this.stopRecord();

        this._saving.saveNext(
            this._currentTask, 
            this._bindedEditor.codeMirror?.getValue() ?? '', 
            this._record.getTaskRecord(this._currentTask));

        this.startRecord();
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
}
