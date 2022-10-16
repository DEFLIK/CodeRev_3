using System;
using System.Collections.Generic;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;
using UserService.Models.Review;
using Task = UserService.DAL.Entities.Task;

namespace UserService.Helpers
{
    public interface ITaskHelper
    {
        TaskSolutionInfo GetTaskSolutionInfo(string taskSolutionId, out string errorString);
        TaskSolutionInfo GetTaskSolutionInfo(Guid taskSolutionId);
        List<TaskSolution> GetTaskSolutions(Guid interviewSolutionId);
        Task GetTask(Guid taskId);
        TaskSolution GetTaskSolution(Guid taskSolutionId);
        bool TryPutTaskSolutionGrade(string taskSolutionId, GradeEnum grade, out string errorString);
    }

    public class TaskHelper : ITaskHelper
    {
        private readonly IDbRepository dbRepository;
        private readonly IUserHelper userHelper;

        public TaskHelper(IDbRepository dbRepository, IUserHelper userHelper)
        {
            this.dbRepository = dbRepository;
            this.userHelper = userHelper;
        }

        public TaskSolutionInfo GetTaskSolutionInfo(string taskSolutionId, out string errorString)
        {
            (var taskSolutionGuid, errorString) = GuidParser.TryParse(taskSolutionId, nameof(taskSolutionId));
            return errorString == null ? GetTaskSolutionInfo(taskSolutionGuid) : null;
        }

        public TaskSolutionInfo GetTaskSolutionInfo(Guid taskSolutionId)
        {
            var taskSolution = GetTaskSolution(taskSolutionId);
            if (taskSolution == null)
                return null;
            
            var userFullName = userHelper.GetFullNameByInterviewSolutionId(taskSolution.InterviewSolutionId);
            if (userFullName == null)
                return null;
            
            return new TaskSolutionInfo
            {
                TaskSolutionId = taskSolution.Id,
                TaskId = taskSolution.TaskId,
                TaskOrder = ' ',
                InterviewSolutionId = taskSolution.InterviewSolutionId,
                FullName = userFullName,
                Grade = taskSolution.Grade,
                IsDone = taskSolution.IsDone
            };
        }

        public List<TaskSolution> GetTaskSolutions(Guid interviewSolutionId)
            => dbRepository.Get<TaskSolution>(t => t.InterviewSolutionId == interviewSolutionId).ToList();

        public Task GetTask(Guid taskId)
            => dbRepository
                .Get<Task>(t => t.Id == taskId)
                .FirstOrDefault();

        public TaskSolution GetTaskSolution(Guid taskSolutionId)
            => dbRepository
                .Get<TaskSolution>(t => t.Id == taskSolutionId)
                .FirstOrDefault();

        public bool TryPutTaskSolutionGrade(string taskSolutionId, GradeEnum grade, out string errorString)
        {
            (var taskSolutionGuid, errorString) = GuidParser.TryParse(taskSolutionId, nameof(taskSolutionId));
            if (errorString != null)
                return false;
            var taskSolution = GetTaskSolution(taskSolutionGuid);
            if (taskSolution == null)
            {
                errorString = $"no {nameof(taskSolution)} with such id";
                return false;
            }
            if (taskSolution.IsDone == false)
            {
                errorString = $"{nameof(taskSolution)} wasn't done yet";
                return false;
            }
            
            taskSolution.Grade = grade;
            dbRepository.SaveChangesAsync().Wait();
            return true;
        }
    }
}