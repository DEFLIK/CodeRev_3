import { Component, EventEmitter, HostListener, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { UserRole } from 'src/app/auth/models/userRole';
import { SessionStorageService } from 'src/app/auth/services/sessionStorage-service/session-storage.service';
import { CandidateCardInfo } from '../../models/candidateCardInfo';
import { CandidateState, CandidateStateKeys } from '../../models/candidateState';
import { ReviewService } from '../../services/review.service';
import { FormControlType } from '../candidate-grade/candidate-grade.component';
import { convertProgrammingLanguageToString, ProgrammingLanguage } from '../../models/programmingLanguage';

@Component({
    selector: 'app-candidate-card',
    templateUrl: './candidate-card.component.html',
    styleUrls: ['./candidate-card.component.less']
})
export class CandidateCardComponent implements OnDestroy, OnInit {
    @Input()
    public candidate!: CandidateCardInfo;
    @Output()
    public clickEvent: EventEmitter<CandidateCardInfo> = new EventEmitter<CandidateCardInfo>();
    public state = CandidateState;
    public isShowingComment = false;
    public isShowingGrade = false;
    public hrGrade!: FormGroup;
    private _unsubscriber = new Subject<void>();
    constructor(
        private _session: SessionStorageService,
        private _review: ReviewService
    ) {}
    public ngOnInit(): void {
        this.hrGrade = new FormGroup(this.getControls());
    }
    public ngOnDestroy(): void {
        this._unsubscriber.next();
    }

    public select(): void {
        switch(this._session.getJWTInfo().role) {
            case(UserRole.interviewer):
                this.clickEvent.emit(this.candidate);
                break;
            case(UserRole.hrManager):
                this.isShowingGrade = true;
                break;
            default:
                this.clickEvent.emit(this.candidate);
                break;
        }
    }

    public showComment(): void {
        this.isShowingComment = true;
    }

    public closeComment(): void {
        this.isShowingComment = false;
    }

    public closeGrade(): void {
        this.isShowingGrade = false;
    }

    public getControls(): FormControlType {
        const res: FormControlType = {
            result: new FormControl()
        };
        res['result'].setValue(this.candidate.interviewResult);
        res['result'].valueChanges
            .pipe(
                takeUntil(this._unsubscriber)
            )
            .subscribe((value: number) => {
                this._review.setInterviewResult(
                    this.candidate.interviewSolutionId,
                    value
                ).subscribe();
            });

        return res;
    }

    public convertProgrammingLanguageToString(language: ProgrammingLanguage): string {
        return convertProgrammingLanguageToString(language);
    }
}
