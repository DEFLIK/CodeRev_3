import { CompilationError } from '../compilationError';

export class ExecutionResultResponse {
    public success?: boolean;
    public output?: string[];
    public errors?: CompilationError[];
}