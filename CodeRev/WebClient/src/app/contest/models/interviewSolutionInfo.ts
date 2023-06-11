import { InterviewSolutionInfoResposne } from './response/interviewSolutionInfo-response';
import { ProgrammingLanguage } from "../../review/models/programmingLanguage";

export class InterviewSolutionInfo {
    public id: string;
    public vacancy: string;
    public interviewText: string;
    public interviewDurationMs: number;
    public isStarted: boolean;
    public startTimeMs: number;
    public endTimeMs: number;
    public isSubmittedByCandidate: boolean;
    public programmingLanguages: ProgrammingLanguage[];
    public isSynchronous: boolean;

    constructor(resp: InterviewSolutionInfoResposne) {
        this.id = resp.id ?? '';
        this.vacancy = resp.vacancy ?? 'none';
        this.interviewText = resp.interviewText ?? '';
        this.interviewDurationMs = resp.interviewDurationMs ?? -1;
        this.isStarted = resp.isStarted ?? false;
        this.startTimeMs = resp.startTimeMs ?? -1;
        this.endTimeMs = resp.endTimeMs ?? -1;
        this.isSubmittedByCandidate = resp.isSubmittedByCandidate ?? false;
        this.programmingLanguages = resp.programmingLanguages ?? [];
        this.isSynchronous = resp.isSynchronous ?? false;
    }
}
