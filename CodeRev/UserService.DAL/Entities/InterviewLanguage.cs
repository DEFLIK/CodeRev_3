using System;
using UserService.DAL.Models.Enums;

namespace UserService.DAL.Entities;

public class InterviewLanguage : BaseEntity // дублирование информации для оптимизации запросов
{
    public Guid InterviewId { get; set; }
    public ProgrammingLanguage ProgrammingLanguage { get; set; }
}