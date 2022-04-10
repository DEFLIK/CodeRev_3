import { CompilationError } from './compilationError';

export class ExecutionResult {
    public success?: boolean;
    public output?: string[];
    public errors?: CompilationError[];
}