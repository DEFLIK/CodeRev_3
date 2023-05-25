import { ProgrammingLanguage } from "../programmingLanguage";

export class MeetInfoResponse {
    public userId?: string;
    public interviewSolutionId?: string;
    public interviewId?: string;
    public firstName?: string;
    public surname?: string;
    public vacancy?: string;
    public tasksCount?: number;
    public programmingLanguages?: ProgrammingLanguage[];
    public isOwnerMeet?: boolean;
}
