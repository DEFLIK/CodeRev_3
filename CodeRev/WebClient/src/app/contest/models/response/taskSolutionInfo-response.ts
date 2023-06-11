import { ProgrammingLanguage } from "../../../review/models/programmingLanguage";

export class TaskSolutionInfoResponse {
    public id?: string;
    public taskOrder?: string;
    public taskText?: string;
    public startCode?: string;
    public isDone?: boolean;
    public runAttemptsLeft?: number;
    public programmingLanguage?: ProgrammingLanguage;
}
