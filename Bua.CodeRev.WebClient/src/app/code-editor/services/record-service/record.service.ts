import { Injectable } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { CodeRecord, CodePlay } from 'codemirror-record/src';
import { RecordInfo } from '../../models/codeRecord';
import { EditorMode } from '../../models/editorMode';
import { CodeStorageService } from '../code-storage-service/code-storage.service';
// declare var CodeRecord: any;

@Injectable({
    providedIn: 'root'
})
export class RecordService {
    public isPlaying: boolean = false;
    private _recorder: any;
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
        console.log(this._codeMirror);

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

            const savedRecord = this._storage.getSavedRecord();
            console.log('mode: review');

            if (!savedRecord) {
                console.log('There is no any recorded data');
    
                return;
            }
            this._player.addOperations(savedRecord);
            
        } else {
            console.log('mode: write');
        }

        console.log('editor binded');
    }

    public playSavedRecord(): void {
        if (!this._player) {
            console.log('no player');
            
            return;
        }

        this._player.speed = 1;
        this.isPlaying = true;

        // this._codeMirror.setValue('');
        this._player.play();

        console.log('playing');
        console.log(JSON.parse(this._storage.getSavedRecord() ?? ''));
    }

    public pauseSavedRecord(): void {
        if (!this._player) {
            console.log('no player');
            
            return;
        }

        this._player.pause();
        this.isPlaying = false;
        console.log('pause');
    }

    public startRecord(): void {
        if (!this._codeMirror) {
            console.log('Unable to load code mirror model');

            return;
        }

        this._recorder = new CodeRecord(this._codeMirror);
        this._recorder.listen();
        this._codeMirror.setValue(this._codeMirror.getValue());
        console.log('record started');
    }

    public stopAndSaveRecord(): void {
        if (!this._recorder) {
            console.log('Recording not started');
            
            return;
        }

        this._codeMirror.off('changes', this._recorder.changesListener); // some override
        this._codeMirror.off('swapDoc', this._recorder.swapDocListener);
        this._codeMirror.off('cursorActivity', this._recorder.cursorActivityListener);

        const record = this._recorder.getRecords() as string;
        if (record.length === 2) {
            console.log('Recording stoped, nothing to save');

            return;
        }

        this._storage.saveRecord(record);

        this._recorder = null;
        console.log('Record saved');
    }
    
    public clear(): void {
        this._player.clear();
    }

    public seek(pos: number): void {
        if (!this._player) {
            console.log('no player');
        }
        if (this.getDuration() === 0) {
            console.log('zero duration');
        }
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

    public getRecords(): RecordInfo {
        return new RecordInfo(JSON.parse(this._storage.getSavedRecord() ?? ''));
    }
}
