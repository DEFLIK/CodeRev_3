import { Injectable } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { CodeRecord, CodePlay } from 'codemirror-record/src';
import { RecordInfo } from '../../models/codeRecord';
import { SaveChunk } from '../../models/saveChunk';
// import { CodeStorageService } from '../storage-service/code-storage.service';

@Injectable({
    providedIn: 'root'
})
export class PlayerService {
    
    public isPlaying: boolean = false;
    private _playSpeed: number = 0.8;
    private _currentRecord?: RecordInfo;
    private _currentSaves: SaveChunk[] = [];
    private _bindedEditor?: CodemirrorComponent;
    private _currentPlayingChunk: number = 0;
    
    private _maxDelayMs: number = 300000;
    private _player: any;
    constructor() { }
    
    public bindEditor(editorComp: CodemirrorComponent): void {
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
        this._bindedEditor = editorComp;
        this._player.on('end', () => {
            console.log('end');

            if (this._currentPlayingChunk + 1 < this._currentSaves.length) {
                // this.selectRecord(this._currentSaves[this._currentPlayingChunk + 1].record);
                // this.play();
                this.seek(this._currentSaves[this._currentPlayingChunk + 1].record.recordStartTime + 2);

                return;
            }

            this.isPlaying = false;
        });
    }

    public selectSavesRecords(saves: SaveChunk[]): void {
        this._currentSaves = saves;
        this._currentPlayingChunk = 0;
        this.selectChunk(0);
        this.seek(saves[0].record.recordStartTime);
    }

    public selectChunk(chunk: number): void {  
        this.clear();
        
        this._currentRecord = this._currentSaves[chunk].record;
        this._currentPlayingChunk = chunk;
        this._bindedEditor?.codeMirror?.setValue(this._currentSaves[chunk - 1]?.code ?? '');
        const record = JSON.stringify(this._currentRecord.record);

        if (record.length > 2) {
            this._player.addOperations(record);
        }
        
        // this.seek(this._currentRecord.recordStartTime + 1);
    }

    public setSpeed(speed: number): void {
        this._playSpeed = speed;
    }

    public play(): void {
        if (!this._player) {
            return;
        }

        this._player.speed = this._playSpeed;
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

    public clear(): void {
        this._player.clear();
    }

    public seek(time: number): void {
        if (!this._player || !this._currentRecord) {
            return;
        }

        console.log(this._currentSaves);
        

        let pos = time - this._currentRecord.recordStartTime;
        let resultChunk = this._currentPlayingChunk;

        if (pos === 0) {
            pos = 1;
        }

        while (pos > this._currentSaves[resultChunk].record.duration && resultChunk + 1 < this._currentSaves.length) {
            // if (this._currentSaves[resultChunk + 1].record.recordStartTime > time) {

            //     return;
            // }

            resultChunk += 1;
            pos = time - this._currentSaves[resultChunk].record.recordStartTime + 1;
        }
        
        while (pos < 0 && resultChunk >= 0) {    
            // if (this._currentSaves[resultChunk - 1].record.recordStartTime + this._currentSaves[resultChunk - 1].record.duration  < time) {

            //     return;
            // }

            resultChunk -= 1;
            pos = time - this._currentSaves[resultChunk].record.recordStartTime + 1;
        } 
        
        if (resultChunk !== this._currentPlayingChunk) {
            this.selectChunk(resultChunk);
        }

        console.log(`seek > chunk: ${this._currentPlayingChunk}, duration: ${this._currentSaves[resultChunk].record.duration}, pos: ${pos}`);

        this.pause();
        console.log(this._player);
        
        this._player.seek(pos);
    }

    public getCurrentTime(): number {
        return this._currentRecord?.recordStartTime + this._player.getCurrentTime();
    }

    public getSaveDuration(): number {
        let res = 0;
        const firstRecord = this._currentSaves[0].record;

        if (this._currentSaves.length > 1) {
            const lastRecord = this._currentSaves[this._currentSaves.length - 1].record;
            res = lastRecord.recordStartTime + lastRecord.duration - firstRecord.recordStartTime;
        } else {
            res = firstRecord.duration;
        }

        return res;
    }

    public getSaveRecords(): RecordInfo[] {
        const res = [];
        const emptyTime = 0;

        for (const save of this._currentSaves) {
            res.push(save.record);
        }

        return res;
    }

    public getDuration(): number {
        if (!this._player) {
            return 1;
        }

        return this._player.getDuration();
    }
}
