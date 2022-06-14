import { Component, Input, OnInit } from '@angular/core';
import { EditorMode } from 'src/app/code-editor/models/editorMode';
import { InterviewSolutionInfo } from '../../models/interviewSolutionInfo';
import { TaskSolutionInfo } from '../../models/taskSolutionInfo';
import { ContestService } from '../../services/contest-service/contest.service';

@Component({
    selector: 'app-task',
    templateUrl: './task.component.html',
    styleUrls: ['./task.component.less']
})
export class TaskComponent implements OnInit {
    @Input()
    public editorMode!: EditorMode;
    public mode = EditorMode;
    public get isSolutionExpired(): boolean {
        return this._contest.isSolutionExpired;
    }
    public taskInfo?: TaskSolutionInfo;
  	constructor(private _contest: ContestService) { }
    public ngOnInit(): void {
        this._contest
            .taskSelected$
            .subscribe(task => this.taskInfo = task);
    }

    public endTask(): void {
        if (!this.taskInfo) {
            return;
        }
        
        this._contest
            .endTask(this.taskInfo?.id)
            .subscribe(resp => {
                if (resp.ok && this.taskInfo) {
                    this.taskInfo.isDone = true;
                }
            });
    }
}
