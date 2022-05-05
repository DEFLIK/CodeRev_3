import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-candidate-grade',
    templateUrl: './candidate-grade.component.html',
    styleUrls: ['./candidate-grade.component.less']
})
export class CandidateGradeComponent implements OnInit {
    public get id(): string {
        return this._solutionId;
    }
    private _solutionId!: string;

    constructor(private _activatedRoute: ActivatedRoute) { }

    public ngOnInit(): void {
        this._solutionId = this._activatedRoute.snapshot.paramMap.get('solutionId') ?? '';
    }

}
