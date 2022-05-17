import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { EntryPoint } from 'src/app/code-editor/models/entryPoint';
import { CodeStorageService } from 'src/app/code-editor/services/code-storage-service/code-storage.service';
import { CompileService } from 'src/app/code-editor/services/compile-service/compile-service.service';
import { RecordService } from 'src/app/code-editor/services/record-service/record.service';
import { ExecutionResult } from '../../models/executionResult';

@Component({
    selector: 'app-controls',
    templateUrl: './controls.component.html',
    styleUrls: ['./controls.component.less']
})
export class ControlsComponent {
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

    constructor(
        private _compiler: CompileService,
        private _record: RecordService,
        private _codeStorage: CodeStorageService) { }

    public bindToEditor(editor: CodemirrorComponent): void {
        this._bindedEditor = editor;
        this._record.bindEditor(editor);
    }
    
    public saveAndRun(): void {
        this._codeStorage.saveCode(this._bindedEditor?.codeMirror?.getValue() ?? '');

        this._compiler
            .execute(
                this._codeStorage.getSavedCode(),
                new EntryPoint(
                    this._codeStorage.entryNamespace, 
                    this._codeStorage.entryClass, 
                    this._codeStorage.entryMethod))
            .subscribe(resp => {
                if (resp.ok) {
                    this._codeStorage.storeOutput(resp.body ?? new ExecutionResult());
                } else {
                    //todo
                }
            });
    }

    public startRecord(): void {
        if (!this._bindedEditor) {
            console.log('Controls not binded to any exist editor');

            return;
        }

        this._record.startRecord();
    }

    public stopAndSaveRecord(): void {
        if (!this._bindedEditor) {
            console.log('Controls not binded to any exist editor');

            return;
        }

        this._record.stopAndSaveRecord();
    }

    public playSavedRecord(): void {
        if (!this._bindedEditor) {
            console.log('Controls not binded to any exist editor');

            return;
        }

        this._record.playSavedRecord();
    }

    public seek(): void {
        console.log('seeking');
        this._record.seek(this.inputForm.get('slider')?.value);
    }

    public clear(): void {
        this._record.clear();
    }

    public getDuration(): void {
        this._record.getDuration();
    }

}
