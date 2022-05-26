import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import * as CodeMirror from 'codemirror';
import { interval } from 'rxjs';
import { HttpService } from 'src/app/global-services/request/http.service';
import { ControlsComponent } from './components/controls/controls.component';
import { OutputComponent } from './components/output/output.component';
import { EditorMode } from './models/editorMode';
import { ExecutionResult } from './models/executionResult';
import { CodeStorageService } from './services/code-storage-service/code-storage.service';
import { RecordService } from './services/record-service/record.service';

type CodeMirrorOptions = {[key: string]: any};

@Component({
    selector: 'app-code-editor',
    templateUrl: './code-editor.component.html',
    styleUrls: ['./code-editor.component.less']
})
export class CodeEditorComponent implements AfterViewInit {
    public editor: HTMLElement | null = document.getElementById('codeEdtior');
    @ViewChild('codeMirror') 
    public codeMirrorCmpt!: CodemirrorComponent;
    @ViewChild('controls') 
    public controlsCmpt!: ControlsComponent;
    @ViewChild('output') 
    public outputCmpt!: OutputComponent;
    public editorMode: EditorMode = EditorMode.review;
    public options: CodeMirrorOptions = {
        lineNumbers: true,
        theme: 'material',
        mode: 'text/x-csharp',
        indentUnit: 4,
        readOnly: this.editorMode === EditorMode.review
    };

    constructor(
        private _record: RecordService,
        private _codeStorage: CodeStorageService
    ) { }

    public ngAfterViewInit(): void {
        this._codeStorage
            .onOutputRefresh$
            .subscribe((result: ExecutionResult) => {
                if (!result.success) {
                    for (const error of result.errors ?? []) {
                        if (error.endChar === error.startChar) {
                            error.startChar ??= 1;
                            error.startChar -= 1;
                        }

                        this.codeMirrorCmpt
                            .codeMirror
                            ?.markText(
                                { line: error.startLine ?? 0, ch:error.startChar ?? 0 }, 
                                { line: error.endLine ?? 0 , ch: error.endChar ?? 1 },
                                { css: 'background-color: yellow' });
                    }
                }

                this.outputCmpt.setOutput(result);
            });

        setTimeout(() => { // Закидываю в конец очереди, чтобы дать модулю CodeMirror подгрузить библиотеку
            if (!this.codeMirrorCmpt.codeMirror) {
                console.log('Failed to load codeMirror component!');

                return;
            }

            
            this.codeMirrorCmpt.codeMirror?.setOption('extraKeys', {
                // eslint-disable-next-line @typescript-eslint/naming-convention
                'Right': () => {},
                // eslint-disable-next-line @typescript-eslint/naming-convention
                'Left': () => {},
                // eslint-disable-next-line @typescript-eslint/naming-convention
                'Up': () => {}, 
                // eslint-disable-next-line @typescript-eslint/naming-convention
                'Down': () => {}
            });

            this.codeMirrorCmpt.codeMirror?.setValue(this._codeStorage.defaultCode);

            this.codeMirrorCmpt.codeMirror?.on('change', () => {
                this.codeMirrorCmpt.codeMirror?.getAllMarks().forEach(marker => marker.clear());
            });
            
            this.controlsCmpt.bindToEditor(this.codeMirrorCmpt);
        });
    }
}
