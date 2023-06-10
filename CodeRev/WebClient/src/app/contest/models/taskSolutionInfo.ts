import { TaskSolutionInfoResponse } from './response/taskSolutionInfo-response';
import { ProgrammingLanguage } from '../../review/models/programmingLanguage';

export class TaskSolutionInfo {
    public id: string;
    public taskOrder: string;
    public taskText: string;
    public startCode: string;
    public isDone: boolean;
    public runAttemptsLeft: number;
    public programmingLanguage: ProgrammingLanguage;

    constructor(resp: TaskSolutionInfoResponse) {
        this.id = resp.id ?? '';
        this.taskOrder = resp.taskOrder ?? '?';
        this.taskText = resp.taskText ?? 'Не удалось получить информацию о задании';
        this.startCode = resp.startCode ?? '';
        this.isDone = resp.isDone ?? false;
        this.runAttemptsLeft = resp.runAttemptsLeft ?? 0;
        this.programmingLanguage = resp.programmingLanguage ?? ProgrammingLanguage.csharp;
    }
}
