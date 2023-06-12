using System;
using System.Collections.Generic;
using UserService.DAL.Models.Enums;

namespace UserService.Models.Interviews;

public class InterviewDto
{
    public Guid Id { get; set; }
    public string Vacancy { get; set; }
    public string InterviewText { get; set; }
    public long InterviewDurationMs { get; set; }
    public Guid CreatedBy { get; set; }
    public List<ProgrammingLanguage> InterviewLanguages { get; set; }
}