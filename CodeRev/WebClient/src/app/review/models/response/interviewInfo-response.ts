import { ProgrammingLanguage } from '../programmingLanguage';

export class InterviewInfoResponse {
    public vacancy?: string;
    public interviewText?: string;
    public interviewDurationMs?: number;
    public id?: string;
    public interviewLanguages?: ProgrammingLanguage[];
}