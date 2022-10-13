import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { InterviewInfo } from '../../models/interviewInfo';
import { Invitation } from '../../models/invitation';
import { ReviewService } from '../../services/review.service';

@Component({
    selector: 'app-candidate-invite',
    templateUrl: './candidate-invite.component.html',
    styleUrls: ['./candidate-invite.component.less']
})
export class CandidateInviteComponent implements OnInit, OnDestroy {
    @Output()
    public closeEvent = new EventEmitter<void>();
    @Input()
    public openEvent!: EventEmitter<void>;
    public isOpen = false;
    public interviews?: InterviewInfo[];
    public links = new Map<string, string | undefined>();
    private _openSub?: Subscription;

    constructor(private _review: ReviewService) { }
    public ngOnDestroy(): void {
        this._openSub?.unsubscribe();
    }
    public ngOnInit(): void {
        this._review
            .getInterviews()
            .subscribe(resp => {
                if (resp.ok && resp.body) {
                    this.interviews = resp.body.map(info => new InterviewInfo(info));
                }
            });
        this._openSub = this.openEvent.subscribe(() => this.open());
    }

    public close(): void {
        this.closeEvent.emit();
        this.isOpen = false;
    }

    public getInvitation(id: string): string | undefined {
        if (this.links.get(id)) {
            return `https://localhost:5001/auth/register/${this.links.get(id)}`;
        }

        return undefined;
    }

    public open(): void {
        this.isOpen = true;
    }

    public create(int: InterviewInfo): void {
        this._review
            .createInvite(new Invitation('candidate', int.id))
            .subscribe(invResp => {
                if (invResp.ok && invResp.body) {
                    this.links.set(int.id, invResp.body.invitation);
                }
            });
    }
}
