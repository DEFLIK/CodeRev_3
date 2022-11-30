import { Draft } from '../draft';

export class SetDraftRequest {
    public interviewSolutionId: string;
    public draft: Draft;

    constructor(intSlnId: string, draft: Draft) {
        this.interviewSolutionId = intSlnId;
        this.draft = draft;
    }
}