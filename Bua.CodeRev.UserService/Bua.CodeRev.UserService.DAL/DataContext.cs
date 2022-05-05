using Bua.CodeRev.UserService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bua.CodeRev.UserService.DAL
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
    }
}