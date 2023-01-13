import { NotificationType } from './notificationType';
import { NotificationResponse } from './response/notification-response';


export class NotificationInfo {
    public text: string;
    public firstName: string;
    public surname: string;

    constructor(resp: NotificationResponse) {
        this.text = this.parseNotificationType(resp.notificationType ?? NotificationType.unknown);
        this.firstName = resp.firstName ?? 'Unnamed';
        this.surname = resp.surname ?? 'Unnamed';
    }

    private parseNotificationType(type: NotificationType): string {
        switch (type) {
            case NotificationType.interviewAdded:
                return 'Добавлено новое интервью';
            case NotificationType.interviewSolutionChecked:
                return 'Интервью проверено';
            case NotificationType.interviewSolutionStarted:
                return 'Решение интервью начато';
            case NotificationType.interviewSolutionSubmitted:
                return 'Решение интервью закончено';
            case NotificationType.taskAdded:
                return 'Добавлена новая задача';
            case NotificationType.userCreated:
                return 'Создан новый пользователь';
            default:
                return 'Неизвестно';
        }
    }
}