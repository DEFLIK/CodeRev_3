import { Component, Input, OnInit } from '@angular/core';
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
    private _bindedEditor?: CodemirrorComponent;

    constructor(
        private _compiler: CompileService,
        private _record: RecordService,
        private _codeStorage: CodeStorageService) { }

    public bindToEditor(editor: CodemirrorComponent): void {
        this._bindedEditor = editor;
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

        this._record.startRecord(this._bindedEditor);
    }

    public stopAndSaveRecord(): void {
        if (!this._bindedEditor) {
            console.log('Controls not binded to any exist editor');

            return;
        }

        this._record.stopAndSaveRecord(this._bindedEditor);
    }

    public playSavedRecord(): void {
        if (!this._bindedEditor) {
            console.log('Controls not binded to any exist editor');

            return;
        }

        this._record.playSavedRecord(this._bindedEditor);
    }

}
