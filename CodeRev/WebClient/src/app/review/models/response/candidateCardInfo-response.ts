
export class CandidateCardInfoResponse {
    public userId?: string;
    public interviewSolutionId?: string;
    public firstName?: string;
    public surname?: string;
    public vacancy?: string;
    public startTimeMs?: number;
    public timeToCheckMs?: number;
    public averageGrade?: number;
    public reviewerComment?: string;
    public doneTasksCount?: number;
    public tasksCount?: number;
    public interviewResult?: number;
    public hasReviewerCheckResult?: boolean;
    public hasHrCheckResult?: boolean;
    public isSubmittedByCandidate?: boolean;
    public isSolutionTimeExpired?: boolean;
    public programmingLanguage?: string;
    public isSynchronous?: boolean;
}
