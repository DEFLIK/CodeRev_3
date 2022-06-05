import { Injectable } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { CodeRecord, CodePlay } from 'codemirror-record/src';
import { ICodeOperation, ICodeRecord, RecordInfo } from '../../models/codeRecord';
import { EditorMode } from '../../models/editorMode';
import { SavingService } from '../saving-service/saving.service';
import { CodeStorageService } from '../storage-service/code-storage.service';
// declare var CodeRecord: any;

@Injectable({
    providedIn: 'root'
})
export class RecordService {
    private _recordsStartTime: Map<string, number> = new Map();
    private _recorders: Map<string, any> = new Map();
    private _codeMirror!: any;

    constructor() {  
    }

    public bindEditor(editorComp: CodemirrorComponent): void {
        if (!editorComp.codeMirror) {
            console.log('cant bind editor');

            return;
        }

        this._codeMirror = editorComp.codeMirror;
    }

    public initRecordersStream(tasksId: string[]): void {
        console.log(tasksId);
        
        for (const task of tasksId) {
            this.startRecord(task);
        }

        console.log(this._recorders);
        
    }

    public changeRecordingTask(taskId: string): void {
        for (const [, recorder] of this._recorders) {      
            if (recorder) {    
                this.stopRecord(taskId);
            }
        }

        this.startRecord(taskId);
        
        // this._codeMirror.on('changes', curRecorder.changesListener);
        // this._codeMirror.on('swapDoc', curRecorder.swapDocListener);
        // this._codeMirror.on('cursorActivity', curRecorder.cursorActivityListener);
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
            console.log('Recording not started');
            
            return;
        }

        this.stopEditorListening(taskId);
    }

    public getTaskRecord(task: string): RecordInfo {
        // return new RecordInfo(JSON.parse(this._storage.getSavedRecord(task) ?? '{}'));
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
}
