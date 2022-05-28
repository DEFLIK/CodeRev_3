import { AfterViewChecked, AfterViewInit, ChangeDetectionStrategy, Component, ComponentFactoryResolver, ElementRef, HostListener, Input, OnDestroy, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { CanvasPos, NgxVideoTimelineComponent, VideoCellType } from 'ngx-video-timeline';
import { interval, Observable, Subscription } from 'rxjs';
import { EntryPoint } from 'src/app/code-editor/models/entryPoint';
import { CodeStorageService } from 'src/app/code-editor/services/code-storage-service/code-storage.service';
import { CompileService } from 'src/app/code-editor/services/compile-service/compile-service.service';
import { RecordService } from 'src/app/code-editor/services/record-service/record.service';
import { IOperationMark, RecordInfo } from '../../models/codeRecord';
import { EditorMode } from '../../models/editorMode';
import { ExecutionResult } from '../../models/executionResult';
import { ExecutionResultResponse } from '../../models/response/executionResult-response';
import { PatchedTimelineComponent } from '../patched-timeline/patched-timeline.component';

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
    public startTime: number = new Date().getTime();
    public get isPlaying(): boolean {
        return this._record.isPlaying;
    }
    // public get width(): number {
    //     console.log(this.timeline?.nativeElement.offsetWidth);
        
    //     return this.timeline?.nativeElement.offsetWidth ?? 0;
    // }
    public get maxValue(): number {
        // console.log('get');
        if (this._bindedEditor) {
            // console.log(this._record.getDuration());

            return this._record.getDuration();
        }

        return 0;
    }
    public inputForm = new FormGroup({
        slider: new FormControl('')
    });
    private _bindedEditor?: CodemirrorComponent;
    private _rangeUpdater?: Subscription;
    private _currentTask?: string;
    private _taskUpdater?: Subscription;

    constructor(
        private _compiler: CompileService,
        private _record: RecordService,
        private _codeStorage: CodeStorageService,
    ) {
    }

    public ngOnDestroy(): void {
        this._rangeUpdater?.unsubscribe();
        this._taskUpdater?.unsubscribe();
    }
    public ngOnInit(): void {
        this.inputForm.get('slider')?.setValue(0);

        this._taskUpdater = this.taskSelected$
            .subscribe(task => {
                this._currentTask = task;

                if (this.editorMode === EditorMode.write) {
                    this._record.setRecordingTask(task);
                }

                if (this.editorMode === EditorMode.review) {
                    this._record.selectTaskRecord(task);
                    this.patchedTimeline.setProperties(
                        this.startTime, 
                        this._record.getDuration(), 
                        this._record.getTaskRecord(task));

                    this._rangeUpdater = interval(100)
                        .subscribe(() => {
                            const currentTime = this._record.getCurrentTime();
                            
                            if (this._record.isPlaying && currentTime >= 0) {
                                this.inputForm.get('slider')?.disable();
                                this.inputForm.get('slider')?.setValue(currentTime);
                                // this.patchedTimeline.timeLineComp!.isPlayClick = true;
                                this.patchedTimeline.setCurrentTime(currentTime);
                            } else {
                                this.inputForm.get('slider')?.enable();
                                // this.patchedTimeline.timeLineComp!.isPlayClick = false;
                            }
                        });
                }
            });
    }

    public bindToEditor(editor: CodemirrorComponent): void {
        this._bindedEditor = editor;
        this._record.bindEditor(editor, this.editorMode);

        if (this.editorMode === EditorMode.review) {
            this.patchedTimeline.buildComponent();
        }
    }

    public saveAndRun(): void {
        if (!this._currentTask) {
            return;
        }

        this._codeStorage.saveCode(this._currentTask, this._bindedEditor?.codeMirror?.getValue() ?? '');

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

        this._record.startTaskRecord(this._currentTask);
    }

    public stopAndSaveRecord(): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this._record.stopAndSaveTaskRecord(this._currentTask);
    }

    public playSavedRecord(): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this._record.play();
    }

    public pauseSavedRecord(): void {
        if (!this._bindedEditor || !this._currentTask) {
            return;
        }

        this._record.pause();
    }

    public seek(time: number): void {
        this._record.seek(time - this.startTime);
    }

    public clear(): void {
        this._record.clear();
    }
}
