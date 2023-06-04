import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import * as CodeMirror from 'codemirror';
import { interval, Observable, Subject, Subscription, takeUntil } from 'rxjs';
import { HttpService } from 'src/app/global-services/request/http.service';
import { ControlsComponent } from './components/controls/controls.component';
import { ToolbarComponent } from './components/toolbar/toolbar.component';
import { EditorMode } from './models/editorMode';
import { ExecutionResult } from './models/executionResult';
// import { CodeStorageService } from './services/storage-service/code-storage.service';
import { RecordService } from './services/record-service/record.service';
import { TaskSolutionInfo } from '../contest/models/taskSolutionInfo';
import { CompileService } from './services/compile-service/compile-service.service';
import { PlayerService } from './services/player-service/player.service';
import { ContestService } from '../contest/services/contest.service';
import { ReviewService } from '../review/services/review.service';
import { ActivatedRoute } from '@angular/router';

export type CodeMirrorOptions = {[key: string]: any};

@Component({
    selector: 'app-code-editor',
    templateUrl: './code-editor.component.html',
    styleUrls: ['./code-editor.component.less'],
    providers: [
        RecordService
    ]
})
export class CodeEditorComponent implements AfterViewInit, OnDestroy {
    public editor: HTMLElement | null = document.getElementById('codeEdtior');
    @ViewChild('codeMirror') 
    public codeMirrorCmpt!: CodemirrorComponent;
    @ViewChild('controls') 
    public controlsCmpt!: ControlsComponent;
    @ViewChild('toolbar') 
    public toolbarCmpt!: ToolbarComponent;
    @Input()
    public editorMode!: EditorMode;
    @Input()
    public taskSelected$!: Observable<TaskSolutionInfo>;
    @Output()
    public codeChanged$ = new EventEmitter<string>();
    public isSync: boolean;
    public solutionId?: string;
    public isHidden = false;
    public types = EditorMode;
    // @Input()
    // public tasks?: string[];
    public options: CodeMirrorOptions = {
        lineNumbers: true,
        theme: 'neat', // 'material',
        mode: 'text/x-csharp',
        indentUnit: 4,
        autofocus: false
    };
    private _unsubscriber = new Subject<void>();

    constructor(
        private _player: PlayerService,
        private _record: RecordService,
        private _compiler: CompileService,
        private _route: ActivatedRoute
    ) { 
        this.isSync = this._route.snapshot.paramMap.get('state') === 'sync';
    }
    public ngOnDestroy(): void {
        this._unsubscriber.next();
    }
    public ngAfterViewInit(): void { 
        this._player.pageOpen
            .pipe(takeUntil(this._unsubscriber))
            .subscribe(() => this.isHidden = false);
        this._player.pageHidden
            .pipe(takeUntil(this._unsubscriber))
            .subscribe(() => this.isHidden = true);
        this._player.execute
            .pipe(takeUntil(this._unsubscriber))
            .subscribe(execRes => {
                this.applyExecution(execRes);
            });

        this.options['readOnly'] = this.editorMode === EditorMode.review;
        
        this._compiler
            .onOutputRefresh$
            .pipe(takeUntil(this._unsubscriber))
            .subscribe((result: ExecutionResult) => {
                this._record.recordExecute(result);
                this.applyExecution(result);
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
                this.codeChanged$.emit(this.codeMirrorCmpt.codeMirror?.getValue());
                this.codeMirrorCmpt.codeMirror?.getAllMarks().forEach(marker => marker.clear());
            });

            this.codeMirrorCmpt.codeMirror.setSize('100%', '100%');
            
            if (!this.isSync) {
                this.controlsCmpt.bindToEditor(this.codeMirrorCmpt);
                this.toolbarCmpt.bindToEditor(this.codeMirrorCmpt);
            }
        });
    }

    public applyExecution(result: ExecutionResult): void {
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
                        { css: 'background-color: #D33030' });
            }
        }

        this.toolbarCmpt.setConsoleOutput(result);
    }
}
