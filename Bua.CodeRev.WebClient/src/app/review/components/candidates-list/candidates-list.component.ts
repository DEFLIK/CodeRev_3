import { Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { CandidateCardInfo } from '../../models/candidateCardInfo';
import { CandidateFitlerCriteria } from '../../models/candidateFilterCriteria';
import { CandidateState } from '../../models/candidateState';
import { CandidateVacancy } from '../../models/candidateVacancy';
import { CandidateCardComponent } from '../candidate-card/candidate-card.component';

@Component({
    selector: 'app-candidates-list',
    templateUrl: './candidates-list.component.html',
    styleUrls: ['./candidates-list.component.less']
})
export class CandidatesListComponent {
    @ViewChildren('candidateCard')
    public deviceCards!: QueryList<CandidateCardComponent>;
    public candidates: CandidateCardInfo[] = this.generateRandomCardsInfo(30);
    public searchForm: FormGroup = new FormGroup({
        serachInput: new FormControl('')
    });
    public filtersForm: FormGroup = new FormGroup({
        ending: new FormControl(''),
        date: new FormControl('new'),
        state: new FormArray([]),
        vacancy: new FormArray([])
    });
    public states: any[] = [];

    public vacancies: any[] = [];
    public get currentTimeMs(): number {
        return Date.now();
    }
    public get searchCriteria(): string {
        return this.searchForm.get('serachInput')?.value;
    }
    public get filterCriteria(): CandidateFitlerCriteria {
        return new CandidateFitlerCriteria(
            this.filtersForm.get('ending')?.value,
            this.filtersForm.get('date')?.value === 'old',
            this.filtersForm.get('state')?.value,
            this.filtersForm.get('vacancy')?.value
        );
    }

    constructor(
        private _router: Router
    ) { 
        const stateVals = Object.values(CandidateState);
        const stateKeys = Object.keys(CandidateState);
        const vacanVals = Object.values(CandidateVacancy);
        const vacanKeys = Object.keys(CandidateVacancy);

        for (let i = 0; i < stateKeys.length; i++) {
            this.states.push({
                name: stateVals[i],
                value: stateKeys[i]
            });

            this.vacancies.push({
                name: vacanVals[i],
                value: vacanKeys[i]
            });
        }
    }

    public selectCard(candidate: CandidateCardInfo): void {
        console.log(candidate);
        this._router.navigateByUrl(`review/grade/${candidate.interviewSolutionId}`);
    }

    public log(): void {
        console.log(this.searchForm.get('serachInput')?.value);
    }

    public onCheckboxChange(e: any, groupName: string): void {
        const group: FormArray = this.filtersForm.get(groupName) as FormArray;
        if (e.target.checked) {
            group.push(new FormControl(e.target.value));
        } else {
            let i: number = 0;
            group.controls.forEach((item: AbstractControl) => {
                if (item.value === e.target.value) {
                    group.removeAt(i);

                    return;
                }
                i++;
            });
        }
    }

    public generateRandomCardsInfo(count: number): CandidateCardInfo[] {
        const result: CandidateCardInfo[] = [];

        for (let i = 0; i < count; i++) {
            const newCardInfo = new CandidateCardInfo();
            newCardInfo.userId = 'test id';
            newCardInfo.averageGrade = this.getRandomGrade(5);
            newCardInfo.fullName = `Testy Test Testovich ${i}`;
            newCardInfo.vacancy = 'Testy developer';
            newCardInfo.interviewSolutionId = `${this.getRandomInt(100000)}`;
            newCardInfo.tasksCount = 3;

            if (Math.random() > 0.65) {
                newCardInfo.doneTasksCount = newCardInfo.tasksCount;
            } else {
                newCardInfo.doneTasksCount = this.getRandomInt(3);
            }

            newCardInfo.timeToCheckMs = Date.now() + 170000 - this.getRandomInt(200000);

            if ( newCardInfo.doneTasksCount === 0 && Math.random() > 0.3) {
                newCardInfo.startTimeMs = -1;
            } else {
                newCardInfo.startTimeMs = Date.now() - 100000 - this.getRandomInt(400000);
            }
            

            result.push(newCardInfo);
        }

        return result;
    }

    private getRandomGrade(max: number): number {
        return Math.floor(Math.random() * max * 10) / 10;
    }

    private getRandomInt(max: number): number {
        return Math.floor(Math.random() * max);
    }

}
