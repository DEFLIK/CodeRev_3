using System.Collections.Generic;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Interfaces;
using UserService.Models.SyncInterviews;

namespace UserService.Helpers
{
    public interface IMeetsHelper
    {
        public IEnumerable<MeetInfoDto> GetMeets();
    }
    
    public class MeetsHelper : IMeetsHelper
    {
        private readonly IDbRepository dbRepository;

        public MeetsHelper(IDbRepository dbRepository)
        {
            this.dbRepository = dbRepository;
        }

        public IEnumerable<MeetInfoDto> GetMeets()
        {
            var tasksPerInterviews = dbRepository.Get<InterviewTask>()
                .ToList()
                .GroupBy(interviewTask => interviewTask.InterviewId)
                .Select(group => (group.Key, group.Count()));
            
            var meets = dbRepository.Get<InterviewSolution>()
                .ToList()
                .Join(dbRepository.Get<Interview>()
                    .Where(interview => interview.IsSynchronous)
                    .ToList(),
                interviewSolution => interviewSolution.InterviewId,
                interview => interview.Id,
                (interviewSolution, interview) => new MeetInfoDto
                {
                    UserId = interviewSolution.UserId,
                    InterviewSolutionId = interviewSolution.Id,
                    InterviewId = interview.Id,
                    Vacancy = interview.Vacancy,
                    ProgrammingLanguage = interview.ProgrammingLanguage,
                });
            
            meets = meets.Join(
                dbRepository.Get<User>()
                    .ToList(), 
                meet => meet.UserId, 
                user => user.Id,
                (meet, user) =>
                {
                    var splitFullName = user.FullName.Split(' ');
                    meet.FirstName = splitFullName.FirstOrDefault();
                    meet.Surname = splitFullName.ElementAtOrDefault(1);
                    return meet;
                });

            meets = meets.Join(
                tasksPerInterviews,
                meet => meet.InterviewId,
                tasksPerInterview => tasksPerInterview.Key,
                (meet, tasksPerInterview) =>
                {
                    meet.TasksCount = tasksPerInterview.Item2;
                    return meet;
                });
            
            return meets;
        }
    }
}