import { NotificationType } from '../notificationType';

export class NotificationResponse {
    public userId?: string;
    public interviewSolutionId?: string;
    public notificationType?: NotificationType;
    public firstName?: string;
    public surname?: string;
    public vacancy?: string;
    public programmingLanguage?: string;
}