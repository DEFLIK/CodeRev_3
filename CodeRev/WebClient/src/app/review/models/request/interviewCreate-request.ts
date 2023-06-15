export class InterviewCreateRequest {
    constructor(
        public vacancy: string,
        public interviewText: string,
        public interviewDurationMs: number,
        public taskIds: string[]
    ) {

    }
}
