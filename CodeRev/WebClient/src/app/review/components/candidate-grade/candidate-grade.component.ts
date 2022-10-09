import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, interval, of, Subject, takeUntil, takeWhile } from 'rxjs';
import { RecordInfo } from 'src/app/code-editor/models/codeRecord';
import { EditorMode } from 'src/app/code-editor/models/editorMode';
import { SaveChunk } from 'src/app/code-editor/models/saveChunk';
import { SavingService } from 'src/app/code-editor/services/saving-service/saving.service';
import { TaskSolutionInfo } from 'src/app/contest/models/taskSolutionInfo';
import { ContestService } from 'src/app/contest/services/contest-service/contest.service';
import { CandidateCardInfo } from '../../models/candidateCardInfo';
import { InterviewSolutionReview } from '../../models/interviewSolutionReview';
import { InterviewSolutionReviewResponse } from '../../models/response/interviewSolutionReview-response';
import { TaskReview } from '../../models/taskReview';
import { ReviewService } from '../../services/review.service';
export type FormControlType = { [key: string]: AbstractControl };

@Component({
    selector: 'app-candidate-grade',
    templateUrl: './candidate-grade.component.html',
    styleUrls: ['./candidate-grade.component.less']
})
export class CandidateGradeComponent implements OnInit, OnDestroy {
    public slnReview?: InterviewSolutionReview;
    // public tasks = ['A','B'];
    public gradesForm!: FormGroup;
    public modes = EditorMode;
    public startTaskId: string = '';

    // public get controls(): FormControlType {
    //     const res: FormControlType = {
    //         resultComment: new FormControl(''),
    //         resultGrade: new FormControl(-1)
    //     };
    //     for (const task of this.slnReview?.taskSolutionsInfos ?? []) {
    //         const control = new FormControl(-1);
    //         res[`${task.taskOrder}`] = control;
    //         // control.valueChanges
    //         //     .pipe(
    //         //         takeUntil(this._endListening)
    //         //     )
    //         //     .subscribe((value: number) => {
    //         //         this._review
    //         //             .setTaskGrade(task.taskId, value)
    //         //             .subscribe();
    //         //     });
    //     }

    //     console.log(res);

    //     return res;
    // }
    public candidate!: CandidateCardInfo;
    public isWatching = false;
    public get isAllLoaded(): boolean {
        return Array.from(this._loadState.values()).every(state => state === true);
    }
    private _loadState: Map<string, boolean> = new Map<string, boolean>();
    private _solutionId!: string;
    private _endListening = new Subject<void>();

    constructor(
        private _activatedRoute: ActivatedRoute,
        private _router: Router,
        private _review: ReviewService,
        private _saving: SavingService,
        private _contest: ContestService
    ) { }
    public ngOnDestroy(): void {
        this._endListening.next();
    }

    public ngOnInit(): void {
        this._solutionId = this._activatedRoute.snapshot.paramMap.get('solutionId') ?? '';

        this._review
            .getSolutionReview(this._solutionId)
            .subscribe({
                next: (resp) => {
                    if (resp.ok && resp.body) {                   
                        this.slnReview = new InterviewSolutionReview(resp.body);
                        this.gradesForm = new FormGroup(this.getControls(this.slnReview));
                        this.gradesForm.get('resultComment')?.setValue(this.slnReview.reviewerComment);
                        this._saving.clearSaves();
                        this._saving.clearCodes();

                        for (const taskSln of this.slnReview.taskSolutionsInfos) {
                            this._loadState.set(taskSln.taskSolutionId, false);
                            this._review
                                .getSaves(taskSln.taskSolutionId)
                                .pipe(
                                    catchError(err => {
                                        this._loadState.set(taskSln.taskSolutionId, true);

                                        // return of();
                                        throw err;
                                    })
                                )
                                .subscribe({
                                    next: saveResp => {
                                        if (saveResp.ok && saveResp.body) {
                                            this._saving.applySaves(taskSln.taskSolutionId, saveResp.body);
                                            this._loadState.set(taskSln.taskSolutionId, true);
                                        }
                                    },
                                    error: () => {}
                                });
                        }
                    }
                }
            });
    }

    public watch(taskId: string): void {
        // this._contest.selectTaskByOrder(taskOrder);
        this.startTaskId = taskId;
        this.isWatching = true;
    }
    
    public saveComment(): void {
        const com = this.gradesForm.get('resultComment')?.value ?? '';

        this._review
            .setInterviewComment(this._solutionId, com)
            .subscribe();
    }

    public back(): void {
        if (this.isWatching) {
            this.isWatching = false;

            return;
        }

        this._router.navigateByUrl('/review');
    }

    private getControls(slnReview: InterviewSolutionReview): FormControlType {
        const res: FormControlType = {
            resultComment: new FormControl(''),
            resultGrade: new FormControl(-1)
        };
        res['resultGrade'].setValue(slnReview.averageGrade);
        res['resultGrade'].valueChanges
            .pipe(
                takeUntil(this._endListening)
            )
            .subscribe((value: number) => {
                this._review
                    .setInterviewGrade(slnReview.interviewSolutionId, value)
                    .subscribe();
            });

        for (const task of slnReview.taskSolutionsInfos ?? []) {
            const control = new FormControl(task.grade);
            res[`${task.taskOrder}`] = control;
            control.valueChanges
                .pipe(
                    takeUntil(this._endListening)
                )
                .subscribe((value: number) => {
                    this._review
                        .setTaskGrade(task.taskSolutionId, value)
                        .subscribe();
                });
        }

        return res;
    }
}
