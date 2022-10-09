namespace UserService.DAL.Entities
{
    public class Task : BaseEntity
    {
        public string TaskText { get; set; }
        public string StartCode { get; set; }
    }
}