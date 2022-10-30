import { Component } from '@angular/core';
import { ReviewService } from 'src/app/review/services/review.service';
import { Draft, IDraftObject } from '../../models/draft';
import { DraftCheckBox } from '../../models/draftCheckBox';
import { DraftText } from '../../models/draftText';
import { SetDraftRequest } from '../../models/request/setDraftRequest';

@Component({
    selector: 'app-draft',
    templateUrl: './draft.component.html',
    styleUrls: ['./draft.component.less']
})
export class DraftComponent {

    public draftObjects?: IDraftObject[];

    constructor(private _reviewService: ReviewService) { 
        _reviewService
            .getInterviewDraft()
            .subscribe(draftResponse => {
                if (!draftResponse.ok) {
                    console.error('Failed getting draft');
                }

                let draftObjects = draftResponse.body?.texts as IDraftObject[];
                draftObjects.concat(draftResponse.body?.checkBoxes as IDraftObject[]);
                draftObjects = draftObjects.sort((a, b) => a.position - b.position);

                this.draftObjects = draftObjects;
            });
    }

    public addCheckBox(): void {
        if (!this.draftObjects) {
            console.error('Draft is not initialized');

            return;
        }

        const newCheckBox = new DraftCheckBox;

        newCheckBox.isChecked = true;
        newCheckBox.value = 'текст';

        this.draftObjects.push(newCheckBox);
    }

    public saveDraft(): void {
        this._reviewService
            .setInterviewDraft(new SetDraftRequest('id', this.toDraft(this.draftObjects ?? [])))
            .subscribe(saveResponse => {
                if (!saveResponse.ok) {
                    console.error('Failed saving draft');
                }
            });
    }

    private toDraft(draftObjects: IDraftObject[]): Draft {
        const resultDraft = new Draft();
        resultDraft.texts = [];
        resultDraft.checkBoxes = [];
        let counter = 0;

        for (const obj of draftObjects) {
            switch (typeof(obj)) {
                case (DraftText.toString()):
                    const text = obj as DraftText;
                    text.position = counter;
                    resultDraft.texts.push(text);
                    break;
                case (DraftCheckBox.toString()):
                    const checkBox = obj as DraftCheckBox;
                    checkBox.position = counter;
                    resultDraft.checkBoxes.push(checkBox);
                    break;
            }
            counter++;
        }

        resultDraft.maxPosition = draftObjects.length;

        return resultDraft;
    }
}
