import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { InterviewInfo } from '../../models/interviewInfo';
import { Invitation } from '../../models/invitation';
import { ReviewService } from '../../services/review.service';
import { UrlRoutes } from '../../../global-services/request/models/url-routes';
import { ProgrammingLanguage, convertProgrammingLanguageToString } from '../../models/programmingLanguage';
import { FormGroup, FormControl, FormArray, AbstractControl } from '@angular/forms';
import { CandidateState } from '../../models/candidateState';

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
    public form: FormGroup = new FormGroup({
        interviewType: new FormControl('async'),
        startDate: new FormControl('2023-01-01'),
        startTime: new FormControl('13:00')
    });
    public searchForm: FormGroup = new FormGroup({
        searchInput: new FormControl('')
    });
    public get searchCriteria(): string {
        return this.searchForm.get('searchInput')?.value;
    }
    public get isSync(): boolean {
        return this.form.get('interviewType')?.value === 'sync';
    }

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
            return `${UrlRoutes.user}/auth/register/${this.links.get(id)}`;
        }

        return undefined;
    }

    public open(): void {
        this.isOpen = true;
    }

    public create(int: InterviewInfo): void {
        console.log(this.form.get('startTime')?.value, this.form.get('startDate')?.value); // TODO
        
        this._review
            .createInvite(new Invitation('candidate', int.id, this.isSync))
            .subscribe(invResp => {
                if (invResp.ok && invResp.body) {
                    this.links.set(int.id, invResp.body.invitation);
                }
            });
    }

    public convertProgrammingLanguageToString(p: ProgrammingLanguage): string {
        return convertProgrammingLanguageToString(p);
    }
}
