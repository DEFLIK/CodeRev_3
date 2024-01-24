import { Component, OnInit, ViewChild } from '@angular/core';
import { TaskInfo } from '../../models/taskInfo';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { CodeMirrorOptions } from 'src/app/code-editor/code-editor.component';
import { FormControl, FormGroup } from '@angular/forms';
import { ReviewService } from '../../services/review.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { InterviewCreateRequest } from '../../models/request/interviewCreate-request';

@Component({
    selector: 'app-task-selection',
    templateUrl: './task-selection.component.html',
    styleUrls: ['./task-selection.component.less']
})
export class TaskSelectionComponent {

    public editor: HTMLElement | null = document.getElementById('codeEdtior');
    @ViewChild('codeMirror')
    public codeMirrorCmpt!: CodemirrorComponent;
    public taskToShow?: TaskInfo;
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
        searchInput: new FormControl('')
    });
    public get searchCriteria(): string {
        return this.searchForm.get('searchInput')?.value ?? '';
    }

    constructor(private _review: ReviewService, private _router: Router, private _snackBar: MatSnackBar) {
        this._review
            .getAllTasks()
            .subscribe(resp => {
                if (resp.ok && resp.body) {
                    for (const task of resp.body) {
                        if (!this.selectedTasks.some(taskInfo => taskInfo.id === task.id)) {
                            this.tasks.push(new TaskInfo(task));
                        }
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

    public getSelectedTasks(): TaskInfo[] {
        return this.selectedTasks;
    }

    public setSelectedTasks(tasks: TaskInfo[]): void {
        for (const task of tasks) {
            this.selectTask(task);
        }
    }
}
