import { CandidateState } from './candidateState';

export class CandidateCardInfo {
    public userId!: string;
    public interviewSolutionId!: string;
    public fullName!: string;
    public vacancy!: string;
    public startTimeMs!: number;
    public timeToCheckMs!: number;
    public averageGrade!: number;
    public reviewerComment!: string;
    public doneTasksCount!: number;
    public tasksCount!: number;

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