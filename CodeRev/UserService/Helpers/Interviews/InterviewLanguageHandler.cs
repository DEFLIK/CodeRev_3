using System;
using System.Collections.Generic;
using System.Linq;
using UserService.DAL.Entities;
using UserService.DAL.Models.Enums;
using UserService.DAL.Models.Interfaces;

namespace UserService.Helpers.Interviews;

public interface IInterviewLanguageHandler
{
    public Dictionary<Guid, List<ProgrammingLanguage>> GetInterviewsWithLanguages();
    public List<ProgrammingLanguage> GetInterviewLanguages(Guid interviewId);
}

public class InterviewLanguageHandler : IInterviewLanguageHandler
{
    private readonly IDbRepository dbRepository;

    public InterviewLanguageHandler(IDbRepository dbRepository)
    {
        this.dbRepository = dbRepository;
    }

    public Dictionary<Guid, List<ProgrammingLanguage>> GetInterviewsWithLanguages()
        => dbRepository.Get<InterviewLanguage>().GroupBy(interviewAndLanguage => interviewAndLanguage.Id,
                interviewAndLanguage => interviewAndLanguage.ProgrammingLanguage)
           .ToDictionary(interviewWithLanguage => interviewWithLanguage.Key,
                interviewWithLanguage => interviewWithLanguage.ToList());

    public List<ProgrammingLanguage> GetInterviewLanguages(Guid interviewId)
        => dbRepository.Get<InterviewLanguage>(interviewLanguage => interviewLanguage.InterviewId == interviewId)
           .Select(interviewLanguage => interviewLanguage.ProgrammingLanguage)
           .ToList();
}