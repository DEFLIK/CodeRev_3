import { HostListener, Injectable, OnDestroy, OnInit } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { CodeRecord, CodePlay } from 'codemirror-record/src';
import { ExtraActions, ExtraActivity, ICodeOperation, ICodeRecord, RecordInfo } from '../../models/codeRecord';
import { EditorMode } from '../../models/editorMode';
import { ExecutionResult } from '../../models/executionResult';
import { SavingService } from '../saving-service/saving.service';
// import { CodeStorageService } from '../storage-service/code-storage.service';
// declare var CodeRecord: any;

@Injectable()
export class RecordService implements OnDestroy {
    private _recordsStartTime: Map<string, number> = new Map();
    private _recorders: Map<string, any> = new Map();
    private _codeMirror!: any;
    private _currentTaskId?: string;
    private _bindedMethod?: () => void;

    constructor() { }

    public enablePageChangesCheck(): void {
        this._bindedMethod = this.onVisibilityChange.bind(this);
        
        document.addEventListener(
            'visibilitychange',
            this._bindedMethod
        );
    }

    public ngOnDestroy(): void {
        if (this._bindedMethod) {
            document.removeEventListener(
                'visibilitychange',
                this._bindedMethod
            );
        }
    }

    public bindEditor(editorComp: CodemirrorComponent): void {
        if (!editorComp.codeMirror) {
            return;
        }

        this._codeMirror = editorComp.codeMirror;
    }

    public initRecordersStream(tasksId: string[]): void {
        for (const task of tasksId) {
            this.startRecord(task);
        }
    }

    public startRecordingTask(taskId: string): void {
        for (const [key, recorder] of this._recorders) {      
            if (recorder) {    
                this.stopRecord(key);
            }
        }
        
        this._currentTaskId = taskId;

        this.startRecord(taskId);
    }

    public startRecord(taskId: string): void {
        if (!this._codeMirror) {
            console.log('Unable to load code mirror model');

            return;
        }

        const recorder = new CodeRecord(this._codeMirror);

        this._recordsStartTime.set(taskId, recorder.initTime);
        this._recorders.set(taskId, recorder);
        this._recorders.get(taskId).listen();
        this._codeMirror.setValue(this._codeMirror.getValue());
    }

    public stopRecord(taskId: string): void {
        if (!this._recorders || !this._recorders.get(taskId)) {
            console.log('Recording not started', taskId);
            
            return;
        }

        this.stopEditorListening(taskId);
    }

    public getTaskRecord(task: string): RecordInfo {
        const recorder = this._recorders.get(task);
        const startTime = this._recordsStartTime.get(task);

        if (!recorder || !startTime) {
            throw new Error('No recorder | startTime');
        }

        const record = recorder.getRecords() as string;
        const recordsModel = JSON.parse(record) as ICodeRecord[];
        this.aproximateRecordInfo(recordsModel);

        return new RecordInfo(recordsModel, startTime);
    }

    public recordExecute(executionResult: ExecutionResult): void {
        if (this._currentTaskId) {
            this._recorders
                .get(this._currentTaskId)
                .recordExtraActivity(new ExtraActivity<ExecutionResult>(
                    ExtraActions.execute,
                    executionResult
                ));
        }
    }

    private stopEditorListening(recorder: any): void {
        this._codeMirror.off('changes', recorder.changesListener);
        this._codeMirror.off('swapDoc', recorder.swapDocListener);
        this._codeMirror.off('cursorActivity', recorder.cursorActivityListener);
    }

    private aproximateRecordInfo(records: ICodeRecord[]): void {
        // Апроксимация первой операции (операция выставления начального кода)
        if (records.length >= 1) {
            switch(typeof records[0].t) {
                case('number'):
                    records[0].t = -1;
                    break;
                default:
                    records[0].t = [-1, -1];
            }
        }
    }

    private onVisibilityChange(): void {
        if (document.hidden && this._currentTaskId) { 
            this._recorders
                .get(this._currentTaskId)
                .recordExtraActivity(new ExtraActivity(
                    ExtraActions.pageHidden
                ));
        } else if (this._currentTaskId) {
            this._recorders
                .get(this._currentTaskId)
                .recordExtraActivity(new ExtraActivity(
                    ExtraActions.pageOpened
                ));
        }
    }
}
