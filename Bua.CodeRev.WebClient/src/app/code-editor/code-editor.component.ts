import { AfterViewInit, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import * as CodeMirror from 'codemirror';
import { interval, Observable, Subject, Subscription } from 'rxjs';
import { HttpService } from 'src/app/global-services/request/http.service';
import { ControlsComponent } from './components/controls/controls.component';
import { OutputComponent } from './components/output/output.component';
import { EditorMode } from './models/editorMode';
import { ExecutionResult } from './models/executionResult';
// import { CodeStorageService } from './services/storage-service/code-storage.service';
import { RecordService } from './services/record-service/record.service';
import { TaskSolutionInfo } from '../contest/models/taskSolutionInfo';
import { CompileService } from './services/compile-service/compile-service.service';

type CodeMirrorOptions = {[key: string]: any};

@Component({
    selector: 'app-code-editor',
    templateUrl: './code-editor.component.html',
    styleUrls: ['./code-editor.component.less']
})
export class CodeEditorComponent implements AfterViewInit, OnDestroy {
    public editor: HTMLElement | null = document.getElementById('codeEdtior');
    @ViewChild('codeMirror') 
    public codeMirrorCmpt!: CodemirrorComponent;
    @ViewChild('controls') 
    public controlsCmpt!: ControlsComponent;
    @ViewChild('output') 
    public outputCmpt!: OutputComponent;
    @Input()
    public editorMode!: EditorMode;
    @Input()
    public taskSelected$!: Observable<TaskSolutionInfo>;
    public types = EditorMode;
    // @Input()
    // public tasks?: string[];
    public options: CodeMirrorOptions = {
        lineNumbers: true,
        theme: 'neat', // 'material',
        mode: 'text/x-csharp',
        indentUnit: 4
    };

    private _outputRefresh?: Subscription;

    constructor(
        private _compiler: CompileService
    ) { }
    public ngOnDestroy(): void {
        this._outputRefresh?.unsubscribe();
    }
    public ngAfterViewInit(): void {    
        this.options['readOnly'] = this.editorMode === EditorMode.review;
        
        this._outputRefresh = this._compiler
            .onOutputRefresh$
            .subscribe((result: ExecutionResult) => {
                if (!result.success) {
                    for (const error of result.errors) {
                        if (error.endChar === error.startChar) {
                            error.startChar ??= 1;
                            error.startChar -= 1;
                        }

                        this.codeMirrorCmpt
                            .codeMirror
                            ?.markText(
                                { line: error.startLine , ch:error.startChar }, 
                                { line: error.endLine , ch: error.endChar },
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

            if (this.editorMode === EditorMode.review) {
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
            }

            this.codeMirrorCmpt.codeMirror.on('change', () => {
                this.codeMirrorCmpt.codeMirror?.getAllMarks().forEach(marker => marker.clear());
            });

            this.codeMirrorCmpt.codeMirror.setSize('100%', '100%');
            
            this.controlsCmpt.bindToEditor(this.codeMirrorCmpt);
            

            // if (this.editorMode === EditorMode.write) {
            //     this._record.initRecordersStream(this.tasks ?? []);
            // }
        });
    }
}
