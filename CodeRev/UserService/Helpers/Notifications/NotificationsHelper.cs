using System;
using System.Collections.Generic;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Helpers.Auth;
using UserService.Helpers.Interviews;
using UserService.Models.Notifications;

namespace UserService.Helpers.Notifications
{
    public interface INotificationsHelper
    {
        List<NotificationDto> GetReadableNotifications(IList<NotificationType> types);
        List<Notification> GetNotifications(IList<NotificationType> types);
    }
    
    public class NotificationsHelper : INotificationsHelper
    {
        private readonly IDbRepository dbRepository;
        private readonly IUserHelper userHelper;
        private readonly IInterviewHelper interviewHelper;
        private readonly IInterviewLanguageHandler interviewLanguageHandler;

        public NotificationsHelper(IDbRepository dbRepository, IUserHelper userHelper, IInterviewHelper interviewHelper, IInterviewLanguageHandler interviewLanguageHandler)
        {
            this.dbRepository = dbRepository;
            this.userHelper = userHelper;
            this.interviewHelper = interviewHelper;
            this.interviewLanguageHandler = interviewLanguageHandler;
        }

        public List<NotificationDto> GetReadableNotifications(IList<NotificationType> types)
            => GetNotifications(types).Select(MapNotificationToDto).ToList();

        public List<Notification> GetNotifications(IList<NotificationType> types)
            => dbRepository
                .Get<Notification>(notification => types.Contains(notification.NotificationType))
                .ToList();

        private NotificationDto MapNotificationToDto(Notification notification)
        {
            var user = userHelper.Get(notification.UserId);
            userHelper.GetFirstNameAndSurname(user, out var firstName, out var surname);

            var interviewId = interviewHelper.GetInterviewSolution(notification.InterviewSolutionId).InterviewId;
            var interview = interviewHelper.GetInterview(interviewId);

            return new NotificationDto
            {
                UserId = notification.UserId,
                InterviewSolutionId = notification.InterviewSolutionId,
                NotificationType = notification.NotificationType,
                FirstName = firstName,
                Surname = surname,
                Vacancy = interview.Vacancy,
                ProgrammingLanguages = interviewLanguageHandler.GetInterviewLanguages(interviewId),
            };
        }
    }
}