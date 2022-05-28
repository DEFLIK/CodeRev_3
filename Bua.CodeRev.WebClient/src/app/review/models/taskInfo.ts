import { TaskInfoResponse } from './response/taskInfo-response';

export class TaskInfo {
    public taskSolutionId: string;
    public taskId: string;
    public interviewSolutionId: string;
    public fullName: string;
    public isDone: boolean;
    public grade: number;

    constructor(resp: TaskInfoResponse) {
        this.taskSolutionId = resp.taskSolutionId ?? '';
        this.taskId = resp.taskId ?? '';
        this.interviewSolutionId = resp.interviewSolutionId ?? '';
        this.fullName = resp.fullName ?? '';
        this.isDone = resp.isDone ?? false;
        this.grade = resp.grade ?? -1;
    }
}