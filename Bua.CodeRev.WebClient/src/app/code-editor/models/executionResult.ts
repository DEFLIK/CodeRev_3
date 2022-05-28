import { CompilationError } from './compilationError';
import { ExecutionResultResponse } from './response/executionResult-response';

export class ExecutionResult {
    public success: boolean;
    public output: string[];
    public errors: CompilationError[];

    constructor(resp: ExecutionResultResponse) {
        this.success = resp.success ?? false;
        this.output = resp.output ?? ['Не был получен ответ с сервера'];
        this.errors = resp.errors ?? [];
    }
}