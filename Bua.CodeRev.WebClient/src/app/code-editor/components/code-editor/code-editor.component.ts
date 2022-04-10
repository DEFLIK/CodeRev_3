import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import * as CodeMirror from 'codemirror';
import { interval } from 'rxjs';
import { RequestService } from 'src/app/global-services/request/request.service';
import { EntryPoint } from '../../models/entryPoint';
import { CompileService } from '../../services/compile-service.service';
import { OutputComponent } from '../output/output.component';

@Component({
    selector: 'app-code-editor',
    templateUrl: './code-editor.component.html',
    styleUrls: ['./code-editor.component.less']
})
export class CodeEditorComponent implements AfterViewInit {
    public editor: HTMLElement | null = document.getElementById('codeEdtior');
    @ViewChild('codeMirror') 
    public codeMirrorCmpt!: CodemirrorComponent;
    @ViewChild('output') 
    public output!: OutputComponent;
    private _namespace: string = 'CodeRev';
    private _class: string = 'Program';
    private _method: string ='Main';
    private _startCode: string = `using System;
using System.Collections.Generic;

namespace CodeRev
{
    public class Program
    {
        public static void Main() {
            Console.WriteLine("Hello bobiks!");
        }
    }
}`;

    constructor(private _compiler: CompileService) { }

    public ngAfterViewInit(): void {
        setTimeout(() => {
            this.codeMirrorCmpt.codeMirror?.setValue(this._startCode);

            this.codeMirrorCmpt.codeMirror?.on('change', () => {
                this.codeMirrorCmpt.codeMirror?.getAllMarks().forEach(marker => marker.clear());
            });
        });
    }
    
    public run(): void {
        this._compiler.execute(
            this.codeMirrorCmpt.codeMirror?.getValue() as string,
            new EntryPoint(this._namespace, this._class, this._method)
        ).subscribe(result => {
            this.output.setOutput(result);

            if (!result.success) {
                for (const error of result.errors ?? []) {
                    this.codeMirrorCmpt
                        .codeMirror
                        ?.markText(
                            { line: error.startLine ?? 0, ch:error.startChar ?? 10 }, 
                            { line: error.endLine ?? 2 , ch: error.endChar ?? 5 },
                            { css: 'background-color: yellow' });
                }
            }
        });
    }
}
