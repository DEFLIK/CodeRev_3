import { Injectable } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { CodeRecord, CodePlay } from 'codemirror-record/src';
import { ICodeOperation, ICodeRecord, RecordInfo } from '../../models/codeRecord';
import { EditorMode } from '../../models/editorMode';
import { CodeStorageService } from '../code-storage-service/code-storage.service';
// declare var CodeRecord: any;

@Injectable({
    providedIn: 'root'
})
export class RecordService {
    public isPlaying: boolean = false;
    private _recorders: Map<string, any> = new Map();
    private _player: any;
    private _maxDelayMs: number = 3000;
    private _playSpeed: number = 0.8;
    private _codeMirror!: any;

    constructor(private _storage: CodeStorageService) { 
        
    }

    public bindEditor(editorComp: CodemirrorComponent, mode: EditorMode): void {
        if (!editorComp.codeMirror) {
            console.log('cant bind editor');

            return;
        }

        this._codeMirror = editorComp.codeMirror;

        if (mode === EditorMode.review) {
            this._player = new CodePlay(editorComp.codeMirror, {
                maxDelay: this._maxDelayMs,
                autoplay: false,
                autofocus: true,
                speed: this._playSpeed,
                extraActivityHandler: (activityRecorded: any): any => {
                    console.log(activityRecorded);
                },
                extraActivityReverter: (activityRecorded: any): any => {
                    console.log(activityRecorded);
                }
            });
            this._player.on('end', () => {
                console.log('end');
                this.isPlaying = false;
            });
        } else {
            console.log('mode: write');
        }

        console.log('editor binded');
    }

    public initRecordersStream(tasks: string[]): void {
        console.log(tasks);
        
        for (const task of tasks) {
            this.startTaskRecord(task);
        }

        console.log(this._recorders);
        
    }

    public selectTaskRecord(task: string): void {
        const newRecord = this._storage.getSavedRecord(task);
        
        this.clear();
        this._player.addOperations(newRecord);
        
        this.seek(0);
        
    }

    public play(): void {
        if (!this._player) {
            return;
        }

        this._player.speed = 1;
        this.isPlaying = true;

        // this._codeMirror.setValue('');
        this._player.play();
    }

    public pause(): void {
        if (!this._player) {
            return;
        }

        this._player.pause();
        this.isPlaying = false;
    }

    public setRecordingTask(task: string): void {
        for (const [t, recorder] of this._recorders) {
            console.log(recorder);
            
            this.stopEditorListening(recorder);
        }

        const curRecorder = this._recorders.get(task);
        
        this._codeMirror.on('changes', curRecorder.changesListener);
        this._codeMirror.on('swapDoc', curRecorder.swapDocListener);
        this._codeMirror.on('cursorActivity', curRecorder.cursorActivityListener);
    }

    public startTaskRecord(task: string): void {
        if (!this._codeMirror) {
            console.log('Unable to load code mirror model');

            return;
        }

        this._recorders.set(task, new CodeRecord(this._codeMirror));
        this._recorders.get(task).listen();
        this._codeMirror.setValue(this._codeMirror.getValue());
    }

    public stopAndSaveTaskRecord(task: string): void {
        if (!this._recorders) {
            console.log('Recording not started');
            
            return;
        }

        const recorder = this._recorders.get(task);

        this.stopEditorListening(task);

        let record = recorder.getRecords() as string;
        if (record.length === 2) {
            return;
        }

        const recordModel = JSON.parse(record) as ICodeRecord[];
        this.aproximateRecordInfo(recordModel);
        record = JSON.stringify(recordModel);

        this._storage.saveTaskRecord(task, record);

        // this._recorders.set(task, null);
    }
    
    public clear(): void {
        this._player.clear();
    }

    public seek(pos: number): void {    
        if (!this._player) {
            return;
        }
        
        if (pos < 1) {
            pos = 1;
        }
        console.log('seek', pos);
        
        this._player.seek(pos);
    }

    public getCurrentTime(): number {
        return this._player.getCurrentTime();
    }

    public getDuration(): number {
        if (!this._player) {
            return 1;
        }

        return this._player.getDuration();
    }

    public getTaskRecord(task: string): RecordInfo {
        return new RecordInfo(JSON.parse(this._storage.getSavedRecord(task) ?? '{}'));
    }

    private stopEditorListening(recorder: any): void {
        this._codeMirror.off('changes', recorder.changesListener); // some override
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
