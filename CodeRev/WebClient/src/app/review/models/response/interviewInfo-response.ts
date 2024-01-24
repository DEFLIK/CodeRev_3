import { ProgrammingLanguage } from '../programmingLanguage';
import { TaskInfo } from '../taskInfo';

export class InterviewInfoResponse {
    public vacancy?: string;
    public interviewText?: string;
    public interviewDurationMs?: number;
    public id?: string;
    public interviewLanguages?: ProgrammingLanguage[];
    public tasks?: TaskInfo[];
}