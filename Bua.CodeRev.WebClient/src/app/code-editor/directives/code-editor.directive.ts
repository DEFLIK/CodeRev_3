import { Directive, ElementRef, Renderer2 } from '@angular/core';
import * as CodeMirror from 'codemirror';

@Directive({
    selector: 'codeEditor'
})
export class CodeEditorDirective {

    private _editor: CodeMirror.Editor;

    constructor (
        public element: ElementRef, 
        private _renderer: Renderer2) {
        this._editor = CodeMirror.fromTextArea(
            this._renderer.selectRootElement('codeEditor'),
            {
                lineNumbers: true, 
                mode: { name: 'javascript' },
                value: 'function myScript(){return 100;}\n'
            }
        );
    }

}
