using System;
using Telegram.Bot;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.Helpers.Auth;
using UserService.Helpers.Interviews;
using UserService.Models.Notifications;
using Task = System.Threading.Tasks.Task;

namespace UserService.Helpers.Notifications;

public class TelegramBotHelper
{
    private const string Token = "5979830396:AAH-U13lmuENQM8SEpfI4SnhgFCZXVCdaFA";
    private const long ChatId = -800633924;
    private readonly TelegramBotClient client;
    private readonly IUserHelper userHelper;
    // private readonly Lazy<IInterviewHelper> interviewHelper;
    private readonly NotificationMassageBuilder builder;

    public TelegramBotHelper(IUserHelper userHelper, 
                             // Lazy<IInterviewHelper> interviewHelper, 
                             NotificationMassageBuilder builder)
    {
        this.userHelper = userHelper;
        // this.interviewHelper = interviewHelper;
        client = new TelegramBotClient(Token);
        this.builder = builder;
    }

    public async Task SendNotification(Notification notification)
    {
        var user = userHelper.Get(notification.UserId);
        userHelper.GetFirstNameAndSurname(user, out var firstName, out var surname);

        // var interviewId = interviewHelper.Value.GetInterviewSolution(notification.InterviewSolutionId).InterviewId;
        // var interview = interviewHelper.Value.GetInterview(interviewId);

        var notificationDto = new NotificationDto
        {
            UserId = notification.UserId,
            InterviewSolutionId = notification.InterviewSolutionId,
            NotificationType = notification.NotificationType,
            FirstName = firstName,
            Surname = surname,
            // Vacancy = interview.Vacancy,
            // ProgrammingLanguage = interview.ProgrammingLanguage,
        };
        
        await client.SendTextMessageAsync(ChatId, NotificationMassageBuilder.Build(notificationDto)).ConfigureAwait(false);
    }
}