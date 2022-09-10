import { EntryPoint } from './entryPoint';

export class ExecutionRequest {
    constructor(
        public code: string, 
        public entryPoint: EntryPoint) {}
}