export class ReviewCommentRequest {
    public reviewerComment: string;
    constructor(comment: string) {
        this.reviewerComment = comment;
    }
}