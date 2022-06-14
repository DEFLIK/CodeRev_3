import { InterviewInfoResponse } from './response/interviewInfo-response';

export class InterviewInfo {
    public vacancy: string;
    public interviewText: string;
    public interviewDurationMs: number;
    public id: string;

    constructor(resp: InterviewInfoResponse) {
        this.vacancy = resp.vacancy ?? 'Без вакансии';
        this.interviewText = resp.interviewText ?? '';
        this.interviewDurationMs = resp.interviewDurationMs ?? -1;
        this.id = resp.id ?? '';
    }

}