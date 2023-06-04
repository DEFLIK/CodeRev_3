import { ExecutionResult } from 'src/app/code-editor/models/executionResult';

export class MeetPeerData {
    public taskIdUpdate?: string;
    public codeUpdate?: string;
    public outputUpdate?: ExecutionResult;
}