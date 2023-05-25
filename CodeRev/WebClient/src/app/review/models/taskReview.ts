import { TaskReviewResponse } from './response/taskReview-response';
import { ProgrammingLanguage } from "./programmingLanguage";

export class TaskReview {
    public taskSolutionId: string;
    public taskId: string;
    public interviewSolutionId: string;
    public fullName: string;
    public isDone: boolean;
    public taskOrder: string;
    public grade: number;
    public runAttemptsLeft: number;
    public programmingLanguage: ProgrammingLanguage;

    constructor(resp: TaskReviewResponse) {
        this.taskSolutionId = resp.taskSolutionId ?? '';
        this.taskId = resp.taskId ?? '';
        this.interviewSolutionId = resp.interviewSolutionId ?? '';
        this.fullName = resp.fullName ?? '';
        this.isDone = resp.isDone ?? false;
        this.taskOrder = resp.taskOrder ?? '?';
        this.grade = resp.grade ?? -1;
        this.runAttemptsLeft = resp.runAttemptsLeft ?? 0;
        this.programmingLanguage = resp.programmingLanguage ?? ProgrammingLanguage.csharp;
    }
}
