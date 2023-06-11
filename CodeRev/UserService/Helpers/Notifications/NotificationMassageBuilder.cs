using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UserService.DAL.Models.Enums;
using UserService.Models.Notifications;

namespace UserService.Helpers.Notifications;

public class NotificationMassageBuilder
{
    public static string Build(NotificationDto notificationDto)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"ФИО: {notificationDto.FirstName} {notificationDto.Surname}");
        sb.AppendLine($"Действие: {ConvertInterview(notificationDto.NotificationType)}");
        if (notificationDto.Vacancy != null)
        {
            sb.AppendLine($"Вакансия: {notificationDto.Vacancy}");
        }
        if (notificationDto.ProgrammingLanguages != null)
        {
            sb.AppendLine($"Языки программирования: {string.Join(", ", notificationDto.ProgrammingLanguages.Select(l => l.ToReadableString()))}");
        }

        return sb.ToString();
    }
    
    private static string ConvertInterview(NotificationType notificationType)
        => notificationType switch
        {
            NotificationType.InterviewAdded => "Добавлено новое интервью",
            NotificationType.InterviewSolutionChecked => "Интервью проверено",
            NotificationType.InterviewSolutionStarted => "Решение интервью начато",
            NotificationType.InterviewSolutionSubmitted => "Решение интервью закончено",
            NotificationType.TaskAdded => "Добавлена новая задача",
            NotificationType.UserCreated => "Создан новый пользователь",
            NotificationType.MeetStarted => "Присоединился к встрече",
            _ => "Неизвестно"
        };
}