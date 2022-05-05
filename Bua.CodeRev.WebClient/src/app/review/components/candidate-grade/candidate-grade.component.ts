import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CandidateCardInfo } from '../../models/candidateCardInfo';
type FormControlType = { [key: string]: AbstractControl };

@Component({
    selector: 'app-candidate-grade',
    templateUrl: './candidate-grade.component.html',
    styleUrls: ['./candidate-grade.component.less']
})
export class CandidateGradeComponent implements OnInit {
    public get id(): string {
        return this._solutionId;
    }
    public get formControls(): FormControlType {
        const res: FormControlType = {
            resultComment: new FormControl(''),
            resultGrade: new FormControl(-1)
        };
        for (const task of this.tasks) {
            res[`${task}`] = new FormControl(-1);
        }

        return res;
    }
    public tasks = ['A','B','C','D','E','F','G'];
    public gradesForm: FormGroup = new FormGroup(this.formControls);
    public candidate!: CandidateCardInfo;
    private _solutionId!: string;

    constructor(
        private _activatedRoute: ActivatedRoute,
        private _router: Router
    ) { }

    public ngOnInit(): void {
        this._solutionId = this._activatedRoute.snapshot.paramMap.get('solutionId') ?? '';
    }

    public log(): void {
        console.log(this.gradesForm);
    }
}
