import { ExecutionResult } from 'src/app/code-editor/models/executionResult';
import { TestsRunResponse } from 'src/app/code-editor/models/response/testsRun-response';

export class MeetPeerData {
    public taskIdUpdate?: string;
    public codeUpdate?: string;
    public consoleUpdate?: ExecutionResult;
    public testsUpdate?: TestsRunResponse;
}