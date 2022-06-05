import { CandidateState } from './candidateState';
import { CandidateCardInfoResponse } from './response/candidateCardInfo-response';

export class CandidateCardInfo {
    public userId: string;
    public interviewSolutionId: string;
    public fullName: string;
    public vacancy: string;
    public startTimeMs: number;
    public timeToCheckMs: number;
    public averageGrade: number;
    public reviewerComment: string;
    public doneTasksCount: number;
    public tasksCount: number;
    public interviewResult: number;

    constructor(resp: CandidateCardInfoResponse) {
        this.userId = resp.userId ?? '';
        this.interviewSolutionId = resp.interviewSolutionId ?? '';
        this.fullName = resp.fullName ?? 'Unnamed';
        this.vacancy = resp.vacancy ?? 'Без вакансии';
        this.startTimeMs = resp.startTimeMs ?? -1;
        this.timeToCheckMs = resp.timeToCheckMs ?? -1;
        this.averageGrade = resp.averageGrade ?? -1;
        this.reviewerComment = resp.reviewerComment ?? 'Без комментария';
        this.doneTasksCount = resp.doneTasksCount ?? -1;
        this.tasksCount = resp.tasksCount ?? -1;
        this.interviewResult = resp.interviewResult ?? -1;
    }

    public getState(): CandidateState {
        if (this.tasksCount === this.doneTasksCount && this.startTimeMs !== -1 && Date.now() < this.timeToCheckMs) {
            return CandidateState.done;
        }

        if (this.tasksCount > this.doneTasksCount && this.startTimeMs !== -1 && Date.now() < this.timeToCheckMs) {
            return CandidateState.inProcess;
        }

        if (this.startTimeMs === -1 && Date.now() < this.timeToCheckMs) {
            return CandidateState.notStarted;
        }

        return CandidateState.skiped;
    }
}