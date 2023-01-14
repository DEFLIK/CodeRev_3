using System;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;

namespace UserService.Helpers.Notifications
{
    public interface INotificationsCreator
    {
        Guid Create(Guid userId, Guid interviewSolutionId, NotificationType type);
    }
    
    public class NotificationsCreator : INotificationsCreator
    {
        private readonly IDbRepository dbRepository;

        public NotificationsCreator(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public Guid Create(Guid userId, Guid interviewSolutionId, NotificationType type)
        {
            var notificationId = Guid.NewGuid();
            
            dbRepository.Add(new Notification
            {
                Id = notificationId,
                UserId = userId,
                InterviewSolutionId = interviewSolutionId,
                NotificationType = type,
                CreationTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            }).Wait();
            
            dbRepository.SaveChangesAsync().Wait();
            
            return notificationId;
        }
    }
}