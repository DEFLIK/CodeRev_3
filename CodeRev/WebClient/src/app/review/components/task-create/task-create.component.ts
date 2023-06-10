import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { TaskInfo } from '../../models/taskInfo';
import { CodeMirrorOptions } from 'src/app/code-editor/code-editor.component';
import { ReviewService } from '../../services/review.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { InterviewCreateRequest } from '../../models/request/interviewCreate-request';
import { TaskCreateRequest } from '../../models/request/taskCreate-request';
import { ProgrammingLanguage } from '../../models/programmingLanguage';


@Component({
    selector: 'app-task-create',
    templateUrl: './task-create.component.html',
    styleUrls: ['./task-create.component.less']
})
export class TaskCreateComponent implements AfterViewInit {
    @ViewChild('codeMirrorTask')
    public taskCodeMirror!: CodemirrorComponent;
    @ViewChild('codeMirrorTests')
    public testsCodeMirror!: CodemirrorComponent;
    public taskName: string = '';
    @ViewChild('testAttempts')
    public testAttempts!: ElementRef;
    public taskText: string = '';
    public selectedLanguage: ProgrammingLanguage = ProgrammingLanguage.csharp;
    public options: CodeMirrorOptions = {
        lineNumbers: true,
        theme: 'neat', // 'material',
        mode: 'text/x-csharp',
        indentUnit: 4,
        autofocus: false,
        // readOnly: 'true'
    };

    constructor(private _review: ReviewService, private _router: Router, private _snackBar: MatSnackBar) {
    }

    public ngAfterViewInit(): void {
        setTimeout(() => {
            this.taskCodeMirror?.codeMirror?.setSize('100%', '100%');
            this.testsCodeMirror?.codeMirror?.setSize('100%', '100%');
        });
    }

    public create(): void {
        const request = new TaskCreateRequest(
            this.taskText,
            this.taskCodeMirror?.codeMirror?.getValue() ?? 'NONE',
            this.taskName,
            this.testsCodeMirror?.codeMirror?.getValue() ?? 'NONE',
            Number.parseInt(this.testAttempts.nativeElement.innerText),
            this.selectedLanguage
        );

        this._review.createTask(request).subscribe(resp => {
            if (resp.ok) {
                this._router.navigateByUrl('/review');
                this._snackBar.open('Задача успешно создана!', 'Ок');
            }
        });
    }

    public selectLanguage(language: ProgrammingLanguage): void {
        this.selectedLanguage = language;
    }
}
