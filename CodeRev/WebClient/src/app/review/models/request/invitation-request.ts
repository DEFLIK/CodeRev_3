import { Invitation } from '../invitation';

export class InvitationRequest {
    public role: string;
    public interviewId: string;
    public isSynchronous: boolean;

    constructor(model: Invitation) {
        this.role = model.role;
        this.interviewId = model.interviewId;
        this.isSynchronous = model.isSynchronous
    }
}
