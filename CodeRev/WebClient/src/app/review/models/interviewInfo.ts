import { ProgrammingLanguage } from './programmingLanguage';
import { InterviewInfoResponse } from './response/interviewInfo-response';
import { TaskInfo } from './taskInfo';

export class InterviewInfo {
    public vacancy: string;
    public interviewText: string;
    public interviewDurationMs: number;
    public id: string;
    public interviewLanguages?: ProgrammingLanguage[];
    public tasks?: TaskInfo[];

    constructor(resp: InterviewInfoResponse) {
        this.vacancy = resp.vacancy ?? 'Без вакансии';
        this.interviewText = resp.interviewText ?? '';
        this.interviewDurationMs = resp.interviewDurationMs ?? -1;
        this.id = resp.id ?? '';
        this.interviewLanguages = resp.interviewLanguages ?? [];
        this.tasks = resp.tasks ?? [];
    }

}