import { Component, EventEmitter, OnInit, Output, QueryList, ViewChildren } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { CandidateCardInfo } from '../../models/candidateCardInfo';
import { CandidateFilterCriteria } from '../../models/candidateFilterCriteria';
import { CandidateState } from '../../models/candidateState';
import { MeetInfo } from '../../models/meetInfo';
import { ReviewService } from '../../services/review.service';
import { CandidateCardComponent } from '../candidate-card/candidate-card.component';
import { MeetFilterCriteria } from "../../models/meetFilterCriteria";
import { convertProgrammingLanguageToString, ProgrammingLanguage } from "../../models/programmingLanguage";

@Component({
    selector: 'app-candidates-list',
    templateUrl: './candidates-list.component.html',
    styleUrls: ['./candidates-list.component.less']
})
export class CandidatesListComponent implements OnInit {
    @ViewChildren('candidateCard')
    public cards!: QueryList<CandidateCardComponent>;
    @Output()
    public inviteEvent = new EventEmitter<void>();
    public candidates?: CandidateCardInfo[];
    public meets?: MeetInfo[];

    public isShowingMeets: boolean = false;
    public searchForm: FormGroup = new FormGroup({
        serachInput: new FormControl('')
    });
    public filtersForm: FormGroup = new FormGroup({
        ending: new FormControl(''),
        myMeetsFirst: new FormControl(''),
        date: new FormControl('new'),
        state: new FormArray([]),
        vacancy: new FormArray([]),
        language: new FormArray([])
    });
    public states: any[] = [];
    public state = CandidateState;

    public vacancies: any[] = [];
    public get currentTimeMs(): number {
        return Date.now();
    }
    public programmingLanguages: string[] = [];
    public get searchCriteria(): string {
        return this.searchForm.get('serachInput')?.value;
    }
    public get filterCandidateCriteria(): CandidateFilterCriteria {
        return new CandidateFilterCriteria(
            this.filtersForm.get('ending')?.value,
            this.filtersForm.get('date')?.value === 'old',
            this.filtersForm.get('state')?.value,
            this.filtersForm.get('vacancy')?.value,
            this.filtersForm.get('language')?.value
        );
    }
    public get filterMeetCriteria(): MeetFilterCriteria {
        return new MeetFilterCriteria(
            this.filtersForm.get('vacancy')?.value,
            this.filtersForm.get('myMeetsFirst')?.value,
            this.filtersForm.get('language')?.value
        );
    }

    constructor(
        private _router: Router,
        private _review: ReviewService
    ) { }
    public ngOnInit(): void {
        this._review
            .getCards()
            .subscribe(resp => {
                console.log(resp);

                return this.candidates = resp;
            });
        this._review
            .getVacancies()
            .subscribe(resp => {
                if (resp.ok && resp.body) {
                    const stateVals = Object.values(CandidateState);
                    const stateKeys = Object.keys(CandidateState);

                    for (let i = 0; i < stateKeys.length; i++) {
                        this.states.push({
                            name: stateVals[i],
                            value: stateKeys[i]
                        });
                    }

                    for (let i = 0; i < resp.body.length; i++) {
                        this.vacancies.push({
                            name: resp.body[i],
                            value: resp.body[i]
                        });
                    }
                }
            });
        this.programmingLanguages = Object.values(ProgrammingLanguage)
            .map(v => Number(v))
            .filter(v => !isNaN(v))
            .map(l => convertProgrammingLanguageToString(l));
        this._review
            .getMeets()
            .subscribe(resp => this.meets = resp);
    }

    public selectCard(candidate: CandidateCardInfo): void {
        this._router.navigateByUrl(`review/grade/${candidate.interviewSolutionId}/async`);
    }

    public selectMeet(meet: MeetInfo): void {
        this._router.navigateByUrl(`review/grade/${meet.interviewSolutionId}/sync`);
    }

    public createInterview(): void {
        this._router.navigateByUrl(`review/create/interview`);
    }

    public invite(): void {
        this.inviteEvent.emit();
    }

    public closeInvite(): void {
    }

    public onCheckboxChange(e: any, groupName: string): void {
        const group: FormArray = this.filtersForm.get(groupName) as FormArray;
        if (e.checked) {
            group.push(new FormControl(e.source.value));
        } else {
            let i: number = 0;
            group.controls.forEach((item: AbstractControl) => {
                if (item.value === e.source.value) {
                    group.removeAt(i);

                    return;
                }
                i++;
            });
        }
    }

    public showMeets(open: boolean): void {
        this.isShowingMeets = open;
    }

    // public generateRandomCardsInfo(count: number): CandidateCardInfo[] {
    //     const result: CandidateCardInfo[] = [];

    //     for (let i = 0; i < count; i++) {
    //         const newCardInfo = new CandidateCardInfo();
    //         newCardInfo.userId = 'test id';
    //         newCardInfo.averageGrade = this.getRandomGrade(5);
    //         newCardInfo.fullName = `Testy Test Testovich ${i}`;
    //         newCardInfo.vacancy = 'Testy developer';
    //         newCardInfo.interviewSolutionId = `${this.getRandomInt(100000)}`;
    //         newCardInfo.tasksCount = 3;

    //         if (Math.random() > 0.65) {
    //             newCardInfo.doneTasksCount = newCardInfo.tasksCount;
    //         } else {
    //             newCardInfo.doneTasksCount = this.getRandomInt(3);
    //         }

    //         newCardInfo.timeToCheckMs = Date.now() + 170000 - this.getRandomInt(200000);

    //         if ( newCardInfo.doneTasksCount === 0 && Math.random() > 0.3) {
    //             newCardInfo.startTimeMs = -1;
    //         } else {
    //             newCardInfo.startTimeMs = Date.now() - 100000 - this.getRandomInt(400000);
    //         }


    //         result.push(newCardInfo);
    //     }

    //     return result;
    // }

    private getRandomGrade(max: number): number {
        return Math.floor(Math.random() * max * 10) / 10;
    }

    private getRandomInt(max: number): number {
        return Math.floor(Math.random() * max);
    }

}
