import { ProgrammingLanguage } from 'src/app/review/models/programmingLanguage';
import { EntryPoint } from './entryPoint';

export class ExecutionRequest {
    constructor(
        public code: string,
        public entryPoint: EntryPoint,
        public programmingLanguage: ProgrammingLanguage) {}
}
