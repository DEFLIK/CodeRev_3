using System;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TrackerService.Hubs;
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
        private TelegramBotHelper telegramBotHelper;
        private IHubContext<NotificationHub> hubContext;

        public NotificationsCreator(IDbRepository dbRepository, 
                                    IHubContext<NotificationHub> hubContext,
                                    TelegramBotHelper telegramBotHelper)
        {
            this.dbRepository = dbRepository;
            this.telegramBotHelper = telegramBotHelper;
            this.hubContext = hubContext;
        }

        public Guid Create(Guid userId, Guid interviewSolutionId, NotificationType type)
        {
            var notificationId = Guid.NewGuid();

            var notification = new Notification
            {
                Id = notificationId,
                UserId = userId,
                InterviewSolutionId = interviewSolutionId,
                NotificationType = type,
                CreationTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            };
            
            dbRepository.Add(notification).Wait();
            
            dbRepository.SaveChangesAsync().Wait();

            telegramBotHelper.SendNotification(notification).ConfigureAwait(false);
            hubContext.Clients.All.SendAsync("SendNotification", JsonConvert.SerializeObject(notification));
            
            return notificationId;
        }
    }
}