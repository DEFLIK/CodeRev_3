export class InterviewCreateRequest {
    constructor(
        public vacancy: string,
        public interviewText: string,
        public interviewDurationMs: number,
        public programmingLanguage: string,
        public isSynchronous: boolean,
        public taskIds: string[]
    ) {
        
    }
}