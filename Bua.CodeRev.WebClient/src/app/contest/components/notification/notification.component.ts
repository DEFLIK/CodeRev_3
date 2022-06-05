import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { InterviewSolutionInfo } from '../../models/interviewSolutionInfo';
import { ContestService } from '../../services/contest-service/contest.service';

@Component({
    selector: 'app-notification',
    templateUrl: './notification.component.html',
    styleUrls: ['./notification.component.less']
})
export class NotificationComponent implements OnInit {
    @Output()
    public startInterview = new EventEmitter<InterviewSolutionInfo>();
    @Output()
    public continueInterview = new EventEmitter<InterviewSolutionInfo>();
    @Input()
    public loadedSlnId?: string;

    public sln?: InterviewSolutionInfo;

    constructor(private _contest: ContestService) { }
    public ngOnInit(): void {
        if (this.loadedSlnId) {

        } else {
            this._contest
                .getInterviewSolutionInfo()
                .subscribe({
                    next: (resp) => {
                        if (resp.ok && resp.body) {
                            this.sln = new InterviewSolutionInfo(resp.body);

                            if (this.sln.isStarted) {
                                this.continueInterview.emit(this.sln);
                            }
                        }
                    }
                });
        }
    }

    public start(): void {
        if (!this.sln) {
            return;
        }

        this.startInterview.emit(this.sln);
    }
}
