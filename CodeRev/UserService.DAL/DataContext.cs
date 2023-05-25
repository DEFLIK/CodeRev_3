using Microsoft.EntityFrameworkCore;
using UserService.DAL.Entities;

namespace UserService.DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions) {}
        
        public DbSet<User> Users { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<InterviewTask> InterviewTasks { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<InterviewSolution> InterviewSolutions { get; set; }
        public DbSet<TaskSolution> TaskSolutions { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<ReviewerDraft> ReviewerDrafts { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<InterviewLanguage> InterviewLanguages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReviewerDraft>()
                .Property(reviewerDraft => reviewerDraft.Draft)
                .HasColumnType("jsonb");
        }
    }
}