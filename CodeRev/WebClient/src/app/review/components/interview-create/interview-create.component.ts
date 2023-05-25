import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { CodeMirrorOptions } from 'src/app/code-editor/code-editor.component';
import { InterviewCreateRequest } from '../../models/request/interviewCreate-request';
import { TaskInfo } from '../../models/taskInfo';
import { ReviewService } from '../../services/review.service';

@Component({
    selector: 'app-interview-create',
    templateUrl: './interview-create.component.html',
    styleUrls: ['./interview-create.component.less']
})
export class InterviewCreateComponent{
    public editor: HTMLElement | null = document.getElementById('codeEdtior');
    @ViewChild('codeMirror')
    public codeMirrorCmpt!: CodemirrorComponent;
    public vacancies: any[] = [];
    public duration: string = '10:00';
    public vacancy: string = '';
    public taskToShow?: TaskInfo;
    public welcomeText: string = '';
    public selectedMode: string = 'async';
    public selectedTasks: TaskInfo[] = [];
    public tasks: TaskInfo[] = [];
    public options: CodeMirrorOptions = {
        lineNumbers: true,
        theme: 'neat', // 'material',
        mode: 'text/x-csharp',
        indentUnit: 4,
        autofocus: false,
        // readOnly: 'true'
    };
    public searchForm: FormGroup = new FormGroup({
        serachInput: new FormControl('')
    });
    public get searchCriteria(): string {
        return this.searchForm.get('serachInput')?.value ?? '';
    }
    public get isSync(): boolean {
        return true;
    }

    constructor(private _review: ReviewService, private _router: Router, private _snackBar: MatSnackBar) {
        this._review
            .getVacancies()
            .subscribe(resp => {
                if (resp.ok && resp.body) {
                    for (let i = 0; i < resp.body.length; i++) {
                        this.vacancies.push({
                            name: resp.body[i],
                            value: resp.body[i]
                        });
                    }
                }
            });

        this._review
            .getAllTasks()
            .subscribe(resp => {
                if (resp.ok && resp.body) {
                    for (const task of resp.body) {
                        this.tasks.push(new TaskInfo(task));
                    }
                }

                this.tasks = [...this.tasks];
            });
    }

    public selectTask(task: TaskInfo): void {
        this.tasks = this.tasks.filter(t => t.id !== task.id);
        this.selectedTasks.push(task);
        this.selectedTasks = [...this.selectedTasks];
    }

    public removeTask(task: TaskInfo): void {
        this.selectedTasks = this.selectedTasks.filter(t => t.id !== task.id);
        this.tasks.push(task);
        this.tasks = [...this.tasks];
    }

    public openTask(task: TaskInfo): void {
        this.taskToShow = task;
        setTimeout(() => {
            this.codeMirrorCmpt?.codeMirror?.setSize('100%', '100%');

            return this.codeMirrorCmpt?.codeMirror?.setValue(task.startCode);
        });
    }

    public create(): void {
        var time = this.duration.split(':').map(s => Number.parseInt(s));
        var hrs = time[0] ?? 0;
        var min = time[1] ?? 0;
        var sec = time[2] ?? 0;
        var ms = (hrs*60*60+min*60+sec)*1000;
        this._review.createInterview(new InterviewCreateRequest(
            this.vacancy,
            this.welcomeText,
            ms,
            this.isSync,
            this.selectedTasks.map(task => task.id)
        )).subscribe(resp => {
            if (resp.ok) {
                this._router.navigateByUrl('/review');
                this._snackBar.open('Интервью успешно создано!', 'Ок');
            }
        });
    }
}
