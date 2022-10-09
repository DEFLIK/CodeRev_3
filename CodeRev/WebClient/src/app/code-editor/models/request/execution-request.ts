import { EntryPoint } from './entryPoint-request';

export class ExecutionRequest {
    constructor(
        public code: string, 
        public entryPoint: EntryPoint) {}
}