import { Injectable } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import * as CodeMirror from 'codemirror';
import { CodeRecord, CodePlay } from 'codemirror-record/src';
import { CodeStorageService } from '../code-storage-service/code-storage.service';
// declare var CodeRecord: any;

@Injectable({
    providedIn: 'root'
})
export class RecordService {
    private _recorder: any;
    private _player: any;
    private _maxDelayMs: number = 3000;
    private _playSpeed: number = 0.8;

    constructor(private _storage: CodeStorageService) { 
    }

    public playSavedRecord(editorComp: CodemirrorComponent): void {
        if (!editorComp.codeMirror) {
            return;
        }

        const savedRecord = this._storage.getSavedRecord();

        if (!savedRecord) {
            console.log('There is no any recorded data');

            return;
        }

        editorComp.codeMirror.setValue('');
        this._player = new CodePlay(editorComp.codeMirror, {
            maxDelay: this._maxDelayMs,
            autoplay: true,
            autofocus: true,
            speed: this._playSpeed,
            extraActivityHandler: (activityRecorded: any): any => {
                console.log(activityRecorded);
            },
            extraActivityReverter: (activityRecorded: any): any => {
                console.log(activityRecorded);
            }
        });
        this._player.addOperations(savedRecord);
        this._player.play();
        this._player.on('end', () => {
            console.log('end');
            this._player.clear();
        });
    }

    public startRecord(editorComp: CodemirrorComponent): void {
        if (!editorComp.codeMirror) {
            console.log('Unable to load code mirror model');

            return;
        }

        this._recorder = new CodeRecord(editorComp.codeMirror);
        this._recorder.listen();
        editorComp.codeMirror?.setValue(editorComp.codeMirror.getValue());
        console.log('record started');
    }

    public stopAndSaveRecord(editorComp: CodemirrorComponent): void {
        if (!this._recorder) {
            console.log('Recording not started');
            
            return;
        }

        editorComp.codeMirror?.off('changes', this._recorder.changesListener); // some override
        editorComp.codeMirror?.off('swapDoc', this._recorder.swapDocListener);
        editorComp.codeMirror?.off('cursorActivity', this._recorder.cursorActivityListener);

        const record = this._recorder.getRecords() as string;
        if (record.length === 2) {
            console.log('Recording stoped, nothing to save');

            return;
        }

        this._storage.saveRecord(record);

        this._recorder = null;
        console.log('Record saved');
    }
    
}
