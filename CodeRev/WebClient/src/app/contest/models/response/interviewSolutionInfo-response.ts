import { ProgrammingLanguage } from "../../../review/models/programmingLanguage";

export class InterviewSolutionInfoResposne {
    public id?: string;
    public vacancy?: string;
    public interviewText?: string;
    public interviewDurationMs?: number;
    public isStarted?: boolean;
    public startTimeMs?: number;
    public endTimeMs?: number;
    public isSubmittedByCandidate?: boolean;
    public programmingLanguages?: ProgrammingLanguage[];
    public isSynchronous?: boolean;
}
