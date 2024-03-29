import { ProgrammingLanguage } from '../programmingLanguage';

export class TaskCreateRequest {
    constructor(
        public taskText: string,
        public startCode: string,
        public name: string,
        public testsCode: string,
        public runAttempts: number,
        public programmingLanguage: ProgrammingLanguage
    ) {}
}
