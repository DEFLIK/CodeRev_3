import { MeetInfoResponse } from './response/meetInfo-response';

export class MeetInfo{
    public userId: string;
    public interviewSolutionId: string;
    public interviewId: string;
    public firstName: string;
    public surname: string;
    public vacancy: string;
    public tasksCount: number;
    public programmingLanguage: string;

    constructor(response: MeetInfoResponse) {
        this.userId = response.userId ?? 'none';
        this.interviewSolutionId = response.interviewSolutionId ?? 'none';
        this.interviewId = response.interviewId ?? 'none';
        this.firstName = response.firstName ?? 'Unnamed';
        this.surname = response.surname ?? 'Unnamed';
        this.vacancy = response.vacancy ?? 'Без вакансии';
        this.tasksCount = response.tasksCount ?? -1;
        this.programmingLanguage = response.programmingLanguage ?? '?';
    }
}