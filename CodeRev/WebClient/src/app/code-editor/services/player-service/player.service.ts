import { EventEmitter, Injectable, Output } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { CodeRecord, CodePlay } from 'codemirror-record/src';
import { ExtraActions, ExtraActivity, ICodeOperation, RecordInfo } from '../../models/codeRecord';
import { ExecutionResult } from '../../models/executionResult';
import { ExecutionResultResponse } from '../../models/response/executionResult-response';
import { SaveChunk } from '../../models/saveChunk';
// import { CodeStorageService } from '../storage-service/code-storage.service';

@Injectable({
    providedIn: 'root'
})
export class PlayerService {
    @Output()
    public pageHidden = new EventEmitter();
    @Output()
    public pageOpen = new EventEmitter();
    @Output()
    public execute = new EventEmitter<ExecutionResult>();
    
    public isPlaying: boolean = false;
    public currentRecord?: RecordInfo;
    private _playSpeed: number = 0.8;
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
            extraActivityHandler: (extra: ExtraActivity): any => {
                switch (extra.action) {
                    case (ExtraActions.pageHidden):
                        this.pageHidden.emit();
                        break;
                    case (ExtraActions.pageOpened):
                        this.pageOpen.emit();
                        break;
                    case (ExtraActions.execute):
                        this.execute
                            .emit(
                                (extra as ExtraActivity<ExecutionResult>).data 
                                ?? new ExecutionResult(new ExecutionResultResponse()));
                        break;
                }
            },
            extraActivityReverter: (extra: ExtraActivity): any => {
                switch (extra.action) {
                    case (ExtraActions.pageHidden):
                        this.pageOpen.emit();
                        break;
                    case (ExtraActions.pageOpened):
                        this.pageHidden.emit();
                        break;
                    case (ExtraActions.execute):
                        const res = new ExecutionResultResponse();
                        res.success = true;
                        res.output = [];
                        this.execute
                            .emit(
                                (new ExecutionResult(res)));
                        break;
                }
            }
        });
        this._bindedEditor = editorComp;
        this._player.on('end', () => {
            if (this._currentPlayingChunk + 1 < this._currentSaves.length) {
                // this.selectRecord(this._currentSaves[this._currentPlayingChunk + 1].record);
                // this.play();
                const stateBefore = this.isPlaying;

                this.seek(this._currentSaves[this._currentPlayingChunk + 1].recordInfo.recordStartTime + 2);

                if (stateBefore) {
                    setTimeout(() => this.play());
                }

                return;
            }

            this.isPlaying = false;
        });
    }

    public selectSavesRecords(saves: SaveChunk[]): void {
        this._currentSaves = saves;
        this._currentPlayingChunk = 0;
        this.selectChunk(0);
        this.seek(saves[0].recordInfo.recordStartTime);
    }

    public selectChunk(chunk: number): void {  
        this.clear();
        
        this.currentRecord = this._currentSaves[chunk].recordInfo;
        this._currentPlayingChunk = chunk;
        this._bindedEditor?.codeMirror?.setValue(this._currentSaves[chunk - 1]?.code ?? '');
        const record = JSON.stringify(this.currentRecord.record);

        if (record.length > 2) {
            this._player.addOperations(record);
        }
        
        this.pageOpen.emit();
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
        if (!this._player || !this.currentRecord) {
            return;
        }
        

        let pos = time - this.currentRecord.recordStartTime;
        let resultChunk = this._currentPlayingChunk;

        if (pos === 0) {
            pos = 1;
        }

        while (pos > this._currentSaves[resultChunk].recordInfo.duration && resultChunk + 1 < this._currentSaves.length) {
            // if (this._currentSaves[resultChunk + 1].record.recordStartTime > time) {

            //     return;
            // }

            resultChunk += 1;
            pos = time - this._currentSaves[resultChunk].recordInfo.recordStartTime + 1;
        }
        
        while (pos < 0 && resultChunk > 0) {    
            // if (this._currentSaves[resultChunk - 1].record.recordStartTime + this._currentSaves[resultChunk - 1].record.duration  < time) {

            //     return;
            // }

            resultChunk -= 1;
            pos = time - this._currentSaves[resultChunk].recordInfo.recordStartTime + 1;
        } 
        
        if (resultChunk !== this._currentPlayingChunk) {
            this.selectChunk(resultChunk);
        }

        console.log(`seek > chunk: ${this._currentPlayingChunk}, duration: ${this._currentSaves[resultChunk].recordInfo.duration}, pos: ${pos}`);

        this.pause();
        
        this._player.seek(pos);
    }

    public getCurrentTime(): number {
        return this.currentRecord?.recordStartTime + this._player.getCurrentTime();
    }

    public getSaveDuration(): number {
        let res = 0;
        const firstRecord = this._currentSaves[0].recordInfo;

        if (this._currentSaves.length > 1) {
            const lastRecord = this._currentSaves[this._currentSaves.length - 1].recordInfo;
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
            res.push(save.recordInfo);
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
