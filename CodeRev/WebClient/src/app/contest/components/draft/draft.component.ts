import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReviewService } from 'src/app/review/services/review.service';
import { Draft } from '../../models/draft';
import { DraftCheckBox } from '../../models/draftCheckBox';
import { SetDraftRequest } from '../../models/request/setDraftRequest';


@Component({
    selector: 'app-draft',
    templateUrl: './draft.component.html',
    styleUrls: ['./draft.component.less']
})
export class DraftComponent {

    public draft?: Draft;

    constructor(private _reviewService: ReviewService, private _activatedRoute: ActivatedRoute) { 
        const id = this._activatedRoute.snapshot.paramMap.get('solutionId') ?? '';

        if (!id) {
            return;
        }

        _reviewService
            .getInterviewDraft(id)
            .subscribe(draftResponse => {
                if (!draftResponse.ok) {
                    console.error('Failed getting draft');
                }

                this.draft = draftResponse.body ?? new Draft();
                this.draft.text ??= '';
            });
    }

    public addCheckBox(): void {
        if (!this.draft?.checkboxes) {
            console.error('Draft checkboxes is not initialized');

            return;
        }

        const newCheckBox = new DraftCheckBox;

        newCheckBox.isChecked = false;
        newCheckBox.value = 'текст';

        this.draft.checkboxes.push(newCheckBox);
    }

    public removeCheckBox(i: number): void {
        if (!this.draft?.checkboxes) {
            console.error('Draft checkboxes is not initialized');

            return;
        }

        this.draft.checkboxes.splice(i, 1);
    }

    public saveDraft(): void {
        const id = this._activatedRoute.snapshot.paramMap.get('solutionId') ?? '';

        this._reviewService
            .setInterviewDraft(new SetDraftRequest(id, this.draft ?? new Draft()))
            .subscribe(saveResponse => {
                if (!saveResponse.ok) {
                    console.error('Failed saving draft');
                }
            });
    }

    public drop(event: CdkDragDrop<string[]>): void {
        if (!this.draft?.checkboxes) {
            console.error('Draft checkboxes is not initialized');

            return;
        }

        moveItemInArray(this.draft?.checkboxes, event.previousIndex, event.currentIndex);
    }
}
