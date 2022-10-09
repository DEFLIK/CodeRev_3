namespace UserService.DAL.Entities
{
    public class Interview : BaseEntity
    {
        public string Vacancy { get; set; }
        public string InterviewText { get; set; }
        public long InterviewDurationMs { get; set; }
    }
}