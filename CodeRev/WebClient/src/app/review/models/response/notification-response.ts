import { NotificationType } from '../notificationType';
import { ProgrammingLanguage } from "../programmingLanguage";

export class NotificationResponse {
    public userId?: string;
    public interviewSolutionId?: string;
    public notificationType?: NotificationType;
    public firstName?: string;
    public surname?: string;
    public vacancy?: string;
    public programmingLanguages?: ProgrammingLanguage[];
}
