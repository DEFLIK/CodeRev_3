import { TaskInfoResponse } from './response/taskInfo-response';

export class TaskInfo {
    public id: string;
    public name: string;
    public taskText: string;
    public startCode: string;
    constructor(resp: TaskInfoResponse) {
        this.id = resp.id ?? 'none';
        this.name = resp.name ?? 'Unnamed';
        this.taskText = resp.taskText ?? 'Текст не найден';
        this.startCode = resp.startCode ?? '';
    }
}