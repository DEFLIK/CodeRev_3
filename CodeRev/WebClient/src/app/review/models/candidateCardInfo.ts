import { CandidateState } from './candidateState';
import { CandidateCardInfoResponse } from './response/candidateCardInfo-response';

export class CandidateCardInfo {
    public userId: string;
    public interviewSolutionId: string;
    public firstName: string;
    public surname: string;
    public vacancy: string;
    public startTimeMs: number;
    public timeToCheckMs: number;
    public averageGrade: number;
    public reviewerComment: string;
    public doneTasksCount: number;
    public tasksCount: number;
    public interviewResult: number;
    public hasReviewerCheckResult: boolean;
    public hasHrCheckResult: boolean;
    public isSubmittedByCandidate: boolean;
    public isSolutionTimeExpired: boolean;

    constructor(resp: CandidateCardInfoResponse) {
        this.userId = resp.userId ?? '';
        this.interviewSolutionId = resp.interviewSolutionId ?? '';
        this.firstName = resp.firstName ?? 'Unnamed';
        this.surname = resp.surname ?? 'Unnamed';
        this.vacancy = resp.vacancy ?? 'Без вакансии';
        this.startTimeMs = resp.startTimeMs ?? -1;
        this.timeToCheckMs = resp.timeToCheckMs ?? -1;
        this.averageGrade = resp.averageGrade ?? -1;
        this.reviewerComment = resp.reviewerComment ?? 'Без комментария';
        this.doneTasksCount = resp.doneTasksCount ?? -1;
        this.tasksCount = resp.tasksCount ?? -1;
        this.interviewResult = resp.interviewResult ?? -1;
        this.isSubmittedByCandidate = resp.isSubmittedByCandidate ?? false;
        this.isSolutionTimeExpired = resp.isSolutionTimeExpired ?? false;
        this.hasReviewerCheckResult = resp.hasReviewerCheckResult ?? false;
        this.hasHrCheckResult = resp.hasReviewerCheckResult ?? false;
    }

    public getState(): CandidateState {
        if (this.hasReviewerCheckResult || this.hasHrCheckResult) {
            return CandidateState.checked;
        }

        if (this.isSubmittedByCandidate) {
            return CandidateState.toCheck;
        }

        if (!this.isSolutionTimeExpired) {
            return CandidateState.inProcess;
        }

        return CandidateState.expired;
    }
}